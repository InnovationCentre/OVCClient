using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace OVCClient.Pages
{

    public sealed partial class HomeStation : Page
    {
        private const int switch_container_pin = 12;
        private const int switch_left_pin = 5;
        private const int switch_right_pin = 25;

        private const int led_red_pin = 26;
        private const int led_blue_pin = 6;
        private const int led_green_pin = 16;
        private const int led_yellow_pin = 13;

        private GpioPin switch_left;
        private GpioPin switch_right;
        private GpioPin switch_container;

        private GpioPin led_red;
        private GpioPin led_blue;
        private GpioPin led_green;
        private GpioPin led_yellow;

        private Timer blinkingTimer;
        private DispatcherTimer dispatcherTimer;
        private Dictionary<GpioPin, Ellipse> map;

        public HomeStation()
        {
            this.InitializeComponent();
            initGPIO();
        }

        private void initGPIO()
        {
            map = new Dictionary<GpioPin, Ellipse>();
            var gpio = GpioController.GetDefault();

            // Show an error if there is no GPIO controller
            if (gpio == null)
            {
                layout_status.Text = "There is no GPIO controller on this device. Program is terminated.";
                return;
            }

            //Init switches
            if (switch_left == null)
            {
                switch_left = gpio.OpenPin(switch_left_pin);
                switch_left.SetDriveMode(GpioPinDriveMode.InputPullUp);
                UpdateSwitchLayoutColor(layout_switch_left, GpioPinValue.High);
            }
            if (switch_right == null)
            {
                switch_right = gpio.OpenPin(switch_right_pin);
                switch_right.SetDriveMode(GpioPinDriveMode.InputPullUp);
                UpdateSwitchLayoutColor(layout_switch_right, GpioPinValue.High);
            }
            if (switch_container == null)
            {
                switch_container = gpio.OpenPin(switch_container_pin);
                switch_container.SetDriveMode(GpioPinDriveMode.InputPullUp);
                UpdateSwitchLayoutColor(layout_switch_container, GpioPinValue.High);
            }

            //Init LEDS and set the output so the LEDs are off
            if (led_red == null)
            {
                led_red = gpio.OpenPin(led_red_pin);
                led_red.SetDriveMode(GpioPinDriveMode.Output);
                led_red.Write(GpioPinValue.Low); //The red light should always be on
            }

            if (led_blue == null)
            {
                led_blue = gpio.OpenPin(led_blue_pin);
                led_blue.SetDriveMode(GpioPinDriveMode.Output);
                led_red.Write(GpioPinValue.High);
            }

            if (led_green == null)
            {
                led_green = gpio.OpenPin(led_green_pin);
                led_green.SetDriveMode(GpioPinDriveMode.Output);
                led_red.Write(GpioPinValue.High);
            }

            if (led_yellow == null)
            {
                led_yellow = gpio.OpenPin(led_yellow_pin);
                led_yellow.SetDriveMode(GpioPinDriveMode.Output);
                led_red.Write(GpioPinValue.High);
            }

            map.Add(switch_left, layout_switch_left);
            map.Add(switch_right, layout_switch_right);
            map.Add(switch_container, layout_switch_container);

            dispatcherTimer = new DispatcherTimer();
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Interval = new TimeSpan(0, 0, 1);

            //Set listeners for the switches. Whenever the value of th inputs change, the corresponding methods are called.
            switch_left.ValueChanged += Switch_ValueChanged;
            switch_left.ValueChanged += CheckParking;
            switch_right.ValueChanged += Switch_ValueChanged;
            switch_right.ValueChanged += CheckParking;
            switch_container.ValueChanged += Switch_ValueChanged;

            layout_status.Text = "Initializing complete, program is running correctly";
        }
        
        private void DispatcherTimer_Tick(object sender, object e)
        {
            //Switch High/Low output for the blue LED so it blinks
            led_blue.Write((led_blue.Read() == GpioPinValue.High) ? GpioPinValue.Low : GpioPinValue.High);
        }

        private void CheckParking(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (switch_right.Read() == GpioPinValue.Low || switch_left.Read() == GpioPinValue.Low) //When only one switch is pressed
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    dispatcherTimer.Start();
                });
            }
            else
            {
                Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    dispatcherTimer.Stop();
                });
            }
        }

        private void Switch_ValueChanged(GpioPin sender, GpioPinValueChangedEventArgs args)
        {
            if (args.Edge == GpioPinEdge.RisingEdge) //When a switch is  pressed
            {
                UpdateSwitchLayoutColor(map[sender], sender.Read()); //Update the GUI with the new status
            }
        }

        private void UpdateSwitchLayoutColor(Ellipse ellipse, GpioPinValue value)
        {
            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
               {
                   SolidColorBrush mySolidColorBrush = new SolidColorBrush();

                   mySolidColorBrush.Color = (value == GpioPinValue.High) ? Color.FromArgb(129, 215, 66, 0) : Color.FromArgb(221, 51, 51, 0); //Set the color of the ellipse to either Red or Green
                   ellipse.Fill = mySolidColorBrush;
               });
        }
    }
}

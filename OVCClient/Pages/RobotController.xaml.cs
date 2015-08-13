using System;
using System.Threading;
using Windows.Devices.Gpio;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;


namespace OVCClient.Pages
{
    public sealed partial class RobotController : Page
    {

        private Timer pingTimer;
        private GpioController gpio;

        private int pingPinNumber = 18;
        private GpioPin pingPin;

        public RobotController()
        {
            initGPIO();
            this.InitializeComponent();

            pingTimer = new Timer(this.PingTimerCallBack, null, 0, 2000);

            logMessage("Program initialization complete");
        }

        public void initGPIO()
        {
            var gpio = GpioController.GetDefault();
            if (gpio == null)
            {
                logMessage("There is no GPIO controller on this device. Program is terminated.");
                return;
            }

            if (pingPin == null)
            {
                pingPin = gpio.OpenPin(pingPinNumber);
                pingPin.SetDriveMode(GpioPinDriveMode.Output);
            }

            logMessage("GPIO init successful");
        }

        private void PingTimerCallBack(object state)
        {
            if (pingPin.Read() == GpioPinValue.High)
            {
                pingPin.Write(GpioPinValue.Low);
            }
            else
            {
                pingPin.Write(GpioPinValue.High);
                logMessage("Ping command send");
            }
        }

        public void logMessage(String message)
        {
            var task = this.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                text_output.Text = message + Environment.NewLine + text_output.Text;
            });
        }

        private void pingToggle_Toggled(object sender, RoutedEventArgs e)
        {
            logMessage("Automatic robot detection turned : " + ((pingToggle.IsOn) ? "on" : "off"));

            if (pingToggle.IsOn)
            {
                pingTimer = new Timer(this.PingTimerCallBack, null, 0, 2000);
            }
            else
            {
                pingTimer.Dispose();
                pingTimer = null;
            }
        }
    }
}

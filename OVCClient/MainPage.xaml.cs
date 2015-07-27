
using Microsoft.WindowsAzure.MobileServices;
using OVCClient.DataObjects;
using OVCClient.Pages;
using System;
using System.Threading.Tasks;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace OVCClient
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        public MainPage()
        {
            this.InitializeComponent();
            MainFrame.Navigate(typeof(SendStatus));

            this.SizeChanged += ResponsivePage_SizeChanged;
        }

        private void ResponsivePage_SizeChanged(object sender, SizeChangedEventArgs e)
        {
           if (e.NewSize.Width > 970)
            {
                VisualStateManager.GoToState(this, "Expanded", true);
            }
            else if (e.NewSize.Width > 650)
            {
                VisualStateManager.GoToState(this, "Compact", true);
            }
            //else
            //{
            //    VisualStateManager.GoToState(this, "UltraCompact", true);
            //}
        }

        private void MessagesOverview_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(MessagesOverview));
        }

        private void SendStatus_Click(object sender, RoutedEventArgs e)
        {
            MainFrame.Navigate(typeof(SendStatus));
        }

        private void HamburgerButton_Click(object sender, RoutedEventArgs e)
        {
            MySplitView.IsPaneOpen = !MySplitView.IsPaneOpen;
        }

        private async void SpeechButton_Click(object sender, RoutedEventArgs e)
        {
            // Create an instance of SpeechRecognizer.
            var speechRecognizer = new Windows.Media.SpeechRecognition.SpeechRecognizer();

            // Compile the dictation grammar by default.
            await speechRecognizer.CompileConstraintsAsync();

            // Start recognition.
            Windows.Media.SpeechRecognition.SpeechRecognitionResult speechRecognitionResult = await speechRecognizer.RecognizeWithUIAsync();

            // Do something with the recognition result.
            var messageDialog = new Windows.UI.Popups.MessageDialog(speechRecognitionResult.Text, "Text spoken");
            await messageDialog.ShowAsync();
        }
    }
}
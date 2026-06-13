using WatchMe.Pages;

namespace WatchMe
{
    public partial class MainPage : ContentPage
    {
        public readonly SplitCameraRecordingPage _recordingPage;
        public readonly SettingsPage _settingsPage;

        public MainPage(SplitCameraRecordingPage recordingPage, SettingsPage settingsPage)
        {
            InitializeComponent();
            _recordingPage = recordingPage;
        }

        private async void OnRecordingPageNav(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_recordingPage);
        }

        private async void OnSettingsPageNav(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_settingsPage);
        }
    }
}

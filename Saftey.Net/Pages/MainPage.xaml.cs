using WatchMe.Config;
using WatchMe.Helpers;
using WatchMe.Pages;
using WatchMe.Persistance.CloudProviders;


#if ANDROID
#endif


namespace WatchMe
{
    public partial class MainPage : ContentPage
    {
        public readonly SplitCameraRecordingPage _recordingPage;
        public readonly SettingsPage _settingsPage;
        public readonly ICloudProviderService _cloudProviderService;

        public MainPage(SplitCameraRecordingPage recordingPage, SettingsPage settingsPage, ICloudProviderService cloudProviderService)
        {
            InitializeComponent();
            _recordingPage = recordingPage;
            _settingsPage = settingsPage;
            _cloudProviderService = cloudProviderService;
        }

        private async void OnRecordingPageNav(object sender, EventArgs e)
        {
            var connectionString = await _cloudProviderService.GetAzureConnectionString();

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                await ToastHelper.CreateToast(WatchMeConstants.Settings_ConnectionStringNotFound_AzureSC);
                return;
            }

            await Navigation.PushAsync(_recordingPage);
        }

        private async void OnSettingsPageNav(object sender, EventArgs e)
        {
            await Navigation.PushAsync(_settingsPage);
        }
    }
}

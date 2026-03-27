using System.Text.RegularExpressions;
using WatchMe.Config;
using WatchMe.Helpers;
using WatchMe.Persistance.CloudProviders;

namespace WatchMe.Pages;

public partial class SettingsPage : ContentPage
{
    public readonly ICloudProviderService _cloudProviderService;
    public readonly IPreferences _preferences;
    private string ASCConnectionString = string.Empty;
    private bool ASCConnStringChanged = false;


    private string PhoneNumber = string.Empty;
    private bool PhoneNumberChanged = false;

    private string PhoneNumberRegex = @"^(\+\d{1,2}\s?)?\(?\d{3}\)?[\s.-]?\d{3}[\s.-]?\d{4}$";

    public SettingsPage(ICloudProviderService cloudProviderService)
    {
        InitializeComponent();
        _cloudProviderService = cloudProviderService;
        _preferences = Preferences.Default;
    }

    private async void OnAzureSCConnChanged(object sender, TextChangedEventArgs e)
    {
        ASCConnectionString = e.NewTextValue;
        ASCConnStringChanged = true;
    }

    private async void OnNotifyPhoneNumberChanged(object sender, TextChangedEventArgs e)
    {
        PhoneNumber = e.NewTextValue;
        PhoneNumberChanged = true;
    }

    private async void OnSettingsPageSave(object sender, EventArgs e)
    {
        if (ASCConnStringChanged)
        {
            await _cloudProviderService.SetAzureConnectionString(ASCConnectionString);
        }
        if (PhoneNumberChanged)
        {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.Sms>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.Sms>();
                if (status != PermissionStatus.Granted)
                {
                    await ToastHelper.CreateToast(WatchMeConstants.Settings_PhoneNumber_Discarded);
                    return;
                }
            }

            if (!ValidatePhoneFormat(PhoneNumber))
            {
                await ToastHelper.CreateToast(WatchMeConstants.Settings_PhoneNumber_NonNumericError);
                return;
            }
            _preferences.Set(WatchMeConstants.PhoneNumberPreferencesKey, PhoneNumber);
        }
        await ToastHelper.CreateToast(WatchMeConstants.Settings_Saved);
        await Navigation.PopAsync();
    }

    private bool ValidatePhoneFormat(string phoneFormat)
    {
        if (string.IsNullOrWhiteSpace(phoneFormat))
        {
            return true;
        }
        var match = Regex.Match(phoneFormat, PhoneNumberRegex, RegexOptions.IgnoreCase);
        return match.Success;
    }
}
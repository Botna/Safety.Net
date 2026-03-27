
#if ANDROID
using Android.Telephony;
#endif
using WatchMe.Config;
using WatchMe.Helpers;

namespace WatchMe.Services
{
    public interface INotificationService
    {
        Task SendTextToConfiguredContact(string messageToSend);
    }
    public class NotificationService : INotificationService
    {
        public async Task SendTextToConfiguredContact(string messageToSend)
        {
            var configuredPhoneNumber = Preferences.Default.Get(WatchMeConstants.PhoneNumberPreferencesKey, "");

            if (configuredPhoneNumber == null)
            {
                await ToastHelper.CreateToast(WatchMeConstants.Settings_PhoneNumber_NotConfigured);
                return;
            }
#if ANDROID
            if (!MauiProgram.ISEMULATED)
            {
                var smsM = SmsManager.Default;
                smsM.SendTextMessage(configuredPhoneNumber, null, messageToSend, null, null);
            }
#endif
        }
    }
}

namespace WatchMe.Config
{
    public static class WatchMeConstants
    {
        public const string Settings_ConnectionStringNotFound_AzureSC = "Your azure storage account connection string is not configured";
        public const string Settings_Saved = "Your settings have been saved";
        public const string Settings_PhoneNumber_NonNumericError = "Please provide an appropriate phone number in format `xxx-xxx-xxxx`";
        public const string Settings_PhoneNumber_NotConfigured = "A valid phone number was not figured for notification.  Skipping";
        public const string Settings_PhoneNumber_Discarded = "Phone Number changes discarded due to invalid permission";

        public const string PhoneNumberPreferencesKey = "EVENTSTART_NOTIFY_PHONENUMBER";
    }
}

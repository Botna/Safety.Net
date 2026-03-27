using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;

namespace UITests;

public class RecordingPageTests : BaseTest
{
    [OneTimeSetUp]
    public void OneTimeSetup()
    {
        var settingsButton = FindUIElement(AutomationConstants.Main_SettingsBtn);
        settingsButton.Click();
        Task.Delay(200).Wait();
        var entry = FindUIElement(AutomationConstants.Settings_AzureSCConnstry_Entry);
        entry.SendKeys("TestValue");

        var saveButton = FindUIElement(AutomationConstants.Settings_SaveButton);
        saveButton.Click();

        Task.Delay(200).Wait();
    }

    [Test]
    public void RecordingPageTests_Loads()
    {
        var settingsButton = FindUIElement(AutomationConstants.Main_RecordingStartBtn);
        settingsButton.Click();
        Task.Delay(500).Wait(); // Wait for the click to register and show up on the screenshot

        App.GetScreenshot().SaveAsFile($"{nameof(RecordingPageTests_Loads)}.png");
    }

    [Test]
    public void RecordingPageTests_RecordsAndSavesOnBack()
    {
        App.Navigate().Back();
        App.Navigate().Back();

        var androidOptions = new AppiumOptions
        {
            AutomationName = "UIAutomator2",
            PlatformName = "Android"
        };

        androidOptions.AddAdditionalAppiumOption(MobileCapabilityType.NoReset, "true");
        androidOptions.AddAdditionalAppiumOption(AndroidMobileCapabilityType.AppPackage, "com.google.android.apps.photos");
        androidOptions.AddAdditionalAppiumOption("appium:avd", "pixel_5_-_ui_test");
        androidOptions.AddAdditionalAppiumOption(AndroidMobileCapabilityType.AppActivity, $"com.google.android.apps.photos.MainActivity");
        var newDriver = new AndroidDriver(androidOptions);
    }

    //[Test]
    //public void SettingsPage_SavesAzureSCConnectionString()
    //{
    //    var settingsButton = FindUIElement(AutomationConstants.Main_SettingsBtn);

    //    // Act
    //    settingsButton.Click();
    //    Task.Delay(500).Wait(); // Wait for the click to register and show up on the screenshot

    //    var entry = FindUIElement(AutomationConstants.Settings_AzureSCConnstry_Entry);
    //    entry.SendKeys("TestValue");

    //    var saveButton = FindUIElement(AutomationConstants.Settings_SaveButton);
    //    saveButton.Click();
    //}
}
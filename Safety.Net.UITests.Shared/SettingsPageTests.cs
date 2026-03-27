using FluentAssertions;
using WatchMe.Config;

namespace UITests;

// This is an example of tests that do not need anything platform specific
public class SettingsPageTests : BaseTest
{
    [Test, Order(1)]
    public void SettignsPage_Loads()
    {
        var settingsButton = FindUIElement(AutomationConstants.Main_SettingsBtn);
        settingsButton.Click();
        Task.Delay(500).Wait(); // Wait for the click to register and show up on the screenshot

        App.GetScreenshot().SaveAsFile($"{nameof(SettignsPage_Loads)}.png");
        App.Navigate().Back();
    }

    [Test, Order(2)]
    public void SettingsPage_SavesAzureSCConnectionString()
    {
        var settingsButton = FindUIElement(AutomationConstants.Main_SettingsBtn);
        settingsButton.Click();
        Task.Delay(500).Wait(); // Wait for the click to register and show up on the screenshot

        var entry = FindUIElement(AutomationConstants.Settings_AzureSCConnstry_Entry);
        entry.SendKeys("TestValue");

        var saveButton = FindUIElement(AutomationConstants.Settings_SaveButton);
        saveButton.Click();

        Task.Delay(500).Wait();

        var toast = FindUIElementByXpath("/hierarchy/android.widget.Toast");

        var toastText = toast.GetAttribute("text");
        toastText.Should().Be(WatchMeConstants.Settings_Saved);

    }

    [Test, Order(3)]
    public void SettingsPage_ChangingPhoneNumber_PromptsForPermission()
    {
        var settingsButton = FindUIElement(AutomationConstants.Main_SettingsBtn);
        settingsButton.Click();
        Task.Delay(500).Wait(); // Wait for the click to register and show up on the screenshot

        var entry = FindUIElement(AutomationConstants.Settings_WatchStarted_PhoneNumber);
        entry.SendKeys("1");

        var saveButton = FindUIElement(AutomationConstants.Settings_SaveButton);
        saveButton.Click();

        Task.Delay(500).Wait();

        var permissionPrompt = FindUIElementByXpath(".//android.widget.Button[@text='Allow']");
        permissionPrompt.Should().NotBeNull();

        App.Navigate().Back();
        var toast = FindUIElementByXpath("/hierarchy/android.widget.Toast");

        var toastText = toast.GetAttribute("text");
        toastText.Should().Be(WatchMeConstants.Settings_PhoneNumber_Discarded);
        App.Navigate().Back();
    }

    [Test, Order(4)]
    public void SettingsPage_ChangingPhoneNumber_AcceptPromptAndSavesWithValidPhone()
    {
        var settingsButton = FindUIElement(AutomationConstants.Main_SettingsBtn);
        settingsButton.Click();
        Task.Delay(500).Wait(); // Wait for the click to register and show up on the screenshot

        var entry = FindUIElement(AutomationConstants.Settings_WatchStarted_PhoneNumber);
        entry.SendKeys("555-555-5555");

        var saveButton = FindUIElement(AutomationConstants.Settings_SaveButton);
        saveButton.Click();

        Task.Delay(500).Wait();

        var permissionPrompt = FindUIElementByXpath(".//android.widget.Button[@text='Allow']");
        permissionPrompt.Should().NotBeNull();

        permissionPrompt.Click();

        var toast = FindUIElementByXpath("/hierarchy/android.widget.Toast");
        var toastText = toast.GetAttribute("text");
        toastText.Should().Be(WatchMeConstants.Settings_Saved);
        App.Navigate().Back();
    }
}
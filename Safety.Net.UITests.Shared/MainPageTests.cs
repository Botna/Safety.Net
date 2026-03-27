using FluentAssertions;
using WatchMe.Config;
namespace UITests;

public class MainPageTests : BaseTest
{
    [Test]
    public void AppLaunches()
    {
        App.GetScreenshot().SaveAsFile($"{nameof(AppLaunches)}.png");
    }

    [Test]
    public void ClickRecord_SettingsNotPresent_ToastsError()
    {
        //Remove existing settings value

        var settingsButton = FindUIElement(AutomationConstants.Main_SettingsBtn);

        settingsButton.Click();
        Task.Delay(500).Wait();

        var entry = FindUIElement(AutomationConstants.Settings_AzureSCConnstry_Entry);
        entry.SendKeys("");

        var saveButton = FindUIElement(AutomationConstants.Settings_SaveButton);
        saveButton.Click();

        var recordingButton = FindUIElement(AutomationConstants.Main_RecordingStartBtn);

        recordingButton.Click();
        Task.Delay(500).Wait();

        var toast = FindUIElementByXpath("/hierarchy/android.widget.Toast");

        var toastText = toast.GetAttribute("text");
        toastText.Should().Be(WatchMeConstants.Settings_ConnectionStringNotFound_AzureSC);
    }
}
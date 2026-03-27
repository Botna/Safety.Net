using WatchMe.Services;


namespace WatchMe;

public partial class SplitCameraRecordingPage : ContentPage
{
    private readonly IOrchestrationService _orchestrationService;

    public SplitCameraRecordingPage(IOrchestrationService orchestrationService)
    {
        InitializeComponent();

        _orchestrationService = orchestrationService;
        Loaded += OnPageLoaded;
    }


    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        try
        {
            _orchestrationService.Initialize(null, BackCameraView);
        }
        catch (Exception ex)
        {
            await DisplayAlertAsync("Permission Denied", ex.Message, "OK");
            return;
        }

        Loaded -= OnPageLoaded;
    }


    private async void StartStopRecording(object sender, EventArgs e)
    {
        var currentState = ((Button)sender).Text;
        if (currentState.Equals("Start Recording"))
        {
            await _orchestrationService.InitiateRecordingProcedure();
            ((Button)sender).Text = "Stop Recording";
        }
        else
        {
            await _orchestrationService.StopRecordingProcedure();
            ((Button)sender).Text = "Start Recording";
        }
    }

    protected override async void OnNavigatingFrom(NavigatingFromEventArgs e)
    {
        await _orchestrationService.StopRecordingProcedure();
    }
}
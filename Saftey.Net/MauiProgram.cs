using CommunityToolkit.Maui;
using Microsoft.Extensions.Logging;

using WatchMe.Extensions;
using WatchMe.Pages;
using WatchMe.Persistance.CloudProviders;
using WatchMe.Persistance.Sqlite;
using WatchMe.Services;

namespace WatchMe
{
    public static class MauiProgram
    {
        public static bool ISEMULATED = false;
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .UseMauiCommunityToolkitCamera()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

            builder.AddPlatformSpecificDependancyInjection();

            //Platform independant
            builder.Services.AddSingleton<MainPage>();
            builder.Services.AddTransient<SplitCameraRecordingPage>();
            builder.Services.AddTransient<SettingsPage>();
            builder.Services.AddTransient<IVideosRepository, VideosRepository>();
            builder.Services.AddTransient<IDatabaseInitializer, DatabaseInitializer>();
            builder.Services.AddTransient<IVideoChunksRepository, VideoChunksRepository>();

            builder.Services.AddTransient<IOrchestrationService, OrchestrationService>();
            builder.Services.AddTransient<ICloudProviderService, AzureService>();
            builder.Services.AddTransient<INotificationService, NotificationService>();

            builder.Services.AddSingleton<VideoUploadForegroundService>();
#if DEBUG
            ISEMULATED = true;
            builder.Logging.AddDebug();
#endif
            return builder.Build();
        }
    }
}

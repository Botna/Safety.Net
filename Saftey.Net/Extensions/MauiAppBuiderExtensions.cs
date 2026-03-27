using WatchMe.Persistance.Implementations;
using WatchMe.Repository;
using WatchMe.Services.ForegroundServices;

namespace WatchMe.Extensions
{
    public static class MauiAppBuiderExtensions
    {
        public static void AddPlatformSpecificDependancyInjection(this MauiAppBuilder builder)
        {

#if ANDROID
            builder.Services.AddTransient<IFileSystemService, AndroidFileSystemService>();
            builder.Services.AddTransient<IForegroundServiceDispatcher, AndroidForegroundServiceDispatcher>();
#endif

#if IOS
            builder.Services.AddTransient<IFileSystemService, IOSFileSystemService>();
#endif
        }
    }
}

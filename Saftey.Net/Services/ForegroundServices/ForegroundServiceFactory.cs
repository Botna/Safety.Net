using WatchMe.Helpers;

namespace WatchMe.Services.ForegroundServices
{
    public static class ForegroundServiceFactory
    {
        public static IForegroundService GetServiceFromEnumBundle(string bundle)
        {
            if (Enum.TryParse<ForegroundServiceEnum>(bundle, out var service))
            {
                switch (service)
                {
                    case ForegroundServiceEnum.VUFS:
                        return GetVUFS();
                    default: throw new Exception("unable to parse foregroundserviceenum to appropriate type");
                }
            }
            else
            {
                throw new Exception("unable to parse foregroundserviceenum to appropriate type");
            }
        }

        private static VideoUploadForegroundService GetVUFS()
        {
            var vufs = CurrentServiceProvider.Services.GetService<VideoUploadForegroundService>();
            return vufs ?? throw new ArgumentNullException(nameof(vufs));
        }
    }

}

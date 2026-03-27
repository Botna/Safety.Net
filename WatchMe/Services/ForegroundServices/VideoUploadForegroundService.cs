using WatchMe.Persistance.CloudProviders;
using WatchMe.Persistance.Sqlite;
using WatchMe.Repository;
using WatchMe.Services.ForegroundServices;

namespace WatchMe.Services
{

    public class VideoUploadForegroundService : IForegroundService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly IVideosRepository _videosRepository;
        private readonly ICloudProviderService _cloudProviderService;

        public VideoUploadForegroundService(IFileSystemService? fileSystemService, IVideosRepository? videosRepository, ICloudProviderService? cloudProviderService)
        {
            _cloudProviderService = cloudProviderService ?? throw new ArgumentNullException(nameof(cloudProviderService));
            _fileSystemService = fileSystemService ?? throw new ArgumentNullException(nameof(fileSystemService));
            _videosRepository = videosRepository ?? throw new ArgumentNullException(nameof(videosRepository)); ;
        }
        public async Task DoWorkAsync()
        {
            var SENTINEL = true;
            while (SENTINEL)
            {
                WaitForNextTick();
                //SENTINEL = false;
                var files = await _videosRepository.GetAllVideosAsync();
                //Spin, pull bytes of currently recording videos, and start uploading htem in ~5 second increments. 
                foreach (var file in files)
                {

                    if (file.TotalBytes != 0 && file.TotalBytes == file.BytesOffloaded)
                    {
                        //Video is finished recording, and we've uploaded all the bytes.

                        //need to handle cleanup here, or possibly after the while loop.
                        continue;
                    }

                    var tempFiles = Directory.GetFiles(FileSystem.Current.CacheDirectory);

                    var bytes = _fileSystemService.GetFileBytesFromCacheDirectory(file.VideoName, file.BytesOffloaded);
                    if (bytes != null && bytes.Length > 0)
                    {
                        SENTINEL = true;
                        await _cloudProviderService.AppendContentToCloud(bytes, file.VideoName);
                        await _videosRepository.UpdateBytesOffLoadedOfVideo(file.Id, file.BytesOffloaded + bytes.Length);
                    }
                }
            }
        }

        public virtual void WaitForNextTick()
        {
            var secondsToSleep = 5;
            Thread.Sleep(secondsToSleep * 1000);
        }

        //We let this service stop by its self after its finished uploading.
        public void StopService() { }
    }
}
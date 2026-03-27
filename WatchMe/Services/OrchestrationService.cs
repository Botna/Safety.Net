using CommunityToolkit.Maui.Views;
using WatchMe.Persistance.Sqlite;
using WatchMe.Persistance.Sqlite.Tables;
using WatchMe.Repository;
using WatchMe.Services.ForegroundServices;

namespace WatchMe.Services
{
    public interface IOrchestrationService
    {
        Task Initialize(CameraView backCameraView);
        Task InitiateRecordingProcedure();
        Task StopRecordingProcedure();
    }

    public class OrchestrationService : IOrchestrationService
    {
        private readonly IFileSystemService _fileSystemService;
        private readonly INotificationService _notificationService;
        private readonly IVideosRepository _videosRepository;
        private readonly IForegroundServiceDispatcher _serviceDispatcher;

        private string _videoTimeStamp;

        private string _backVideoFileName;
        private CameraView _backCameraView;

        public OrchestrationService(IFileSystemService fileSystemService, INotificationService notificationService, IDatabaseInitializer databaseInitializer,
            IVideosRepository videosRepository, IForegroundServiceDispatcher serviceDispatcher)
        {
            _fileSystemService = fileSystemService;
            _notificationService = notificationService;
            _videosRepository = videosRepository;
            _serviceDispatcher = serviceDispatcher;

            databaseInitializer.Init();
        }

        public async Task Initialize(CameraView back)
        {
            _videoTimeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");
            _backVideoFileName = $"Back_{_videoTimeStamp}.mp4";
            _backCameraView = back;


            var cameraRequest = await Permissions.RequestAsync<Permissions.Camera>();
            var microphoneRequest = await Permissions.RequestAsync<Permissions.Microphone>();
            if (cameraRequest != PermissionStatus.Granted || microphoneRequest != PermissionStatus.Granted)
            {
                throw new PermissionException("Camera permission is required to use this feature.");
            }

            var availableCameras = await _backCameraView.GetAvailableCameras(CancellationToken.None);
            _backCameraView.SelectedCamera = availableCameras.FirstOrDefault();

            await _backCameraView.StartCameraPreview(CancellationToken.None);

            if (MauiProgram.ISEMULATED)
            {
                //clean up videos in our SQLLITE
                var allVideos = await _videosRepository.GetAllVideosAsync();
                await _videosRepository.DeleteVideosAsync(allVideos.ToArray());

                //clean up videos in our cache directory
                var files = Directory.GetFiles(FileSystem.Current.CacheDirectory);
                var mp4s = files.Where(x => x.Contains("mp4", StringComparison.InvariantCultureIgnoreCase)).ToList();
                foreach (var mp4 in mp4s)
                {
                    File.Delete(mp4);
                }
            }
            //dont start it here yet till we have at max one instance fixed.
            //_serviceDispatcher.StartVUFS();
        }

        public async Task InitiateRecordingProcedure()
        {
            if (!MauiProgram.ISEMULATED)
            {
                var message = "Andrew just started a WatchMe Routine. Click here to watch along: https://www.youtube.com/watch?v=dQw4w9WgXcQ";
                await _notificationService.SendTextToConfiguredContact(message);

            }

            await StartRecordingAsync(_backCameraView, _backVideoFileName);
        }

        private async Task StartRecordingAsync(CameraView camera, string filename)
        {
            camera.ImageCaptureResolution = camera.SelectedCamera.SupportedResolutions.Last();

            var sizes = camera.SelectedCamera.SupportedResolutions;
            Size sizeToUse;

            if (MauiProgram.ISEMULATED)
            {
                sizeToUse = FindSmallestSize(sizes.ToList());
            }
            else
            {
                sizeToUse = sizes.First(x => x.Width == 1920 && x.Height == 1080);
            }

            camera.ImageCaptureResolution = sizeToUse;
            await camera.StartVideoRecording(filename);

            await _videosRepository.InsertVideosAsync(new Videos()
            {
                VideoName = filename,
                VideoState = VideoStates.Recording.ToString(),
                CreatedAt = DateTime.UtcNow
            });

            _serviceDispatcher.StartVUFS();
        }

        private Size FindSmallestSize(List<Size> sizes)
        {
            return sizes.MinBy(size => size.Width * size.Height);
        }


        public async Task StopRecordingProcedure()
        {
            var backMemStream = await _backCameraView.StopVideoRecording(CancellationToken.None);

            if (backMemStream == Stream.Null) return;

            if (backMemStream != Stream.Null)
            {
                var videoTimeStamp = DateTime.UtcNow.ToString("yyyyMMddHHmmssffff");
                await _fileSystemService.MoveVideoToGallery((MemoryStream)backMemStream, "Back_" + videoTimeStamp + ".mp4");
            }

            var backVideo = await _videosRepository.GetVideosByVideoName(_backVideoFileName);
            backVideo.TotalBytes = backMemStream.Length;
            backVideo.VideoState = VideoStates.Finished.ToString();

            await _videosRepository.UpdateTotalBytesOfVideo(backVideo.Id, backVideo.TotalBytes);
            await _videosRepository.UpdateStateOfVideo(backVideo.Id, VideoStates.Finished);
        }
    }
}

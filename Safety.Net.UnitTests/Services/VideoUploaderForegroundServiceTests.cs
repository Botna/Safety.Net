using Moq;
using WatchMe.Persistance.CloudProviders;
using WatchMe.Persistance.Sqlite;
using WatchMe.Persistance.Sqlite.Tables;
using WatchMe.Repository;
using WatchMe.Services;

namespace WatchMe.UnitTests.Services
{
    public class VideoUploaderForegroundServiceTests
    {
        private readonly Mock<IFileSystemService> _mockFileSystemService;
        private readonly Mock<IVideosRepository> _mockVideoRepository;
        private readonly Mock<ICloudProviderService> _mockCloudProviderRepository;
        private readonly Mock<VideoUploadForegroundService> _videoUploadForegroundService;

        public VideoUploaderForegroundServiceTests()
        {
            _mockFileSystemService = new Mock<IFileSystemService>(MockBehavior.Strict);
            _mockVideoRepository = new Mock<IVideosRepository>(MockBehavior.Strict);
            _mockCloudProviderRepository = new Mock<ICloudProviderService>(MockBehavior.Strict);

            _videoUploadForegroundService = new Mock<VideoUploadForegroundService>(_mockFileSystemService.Object, _mockVideoRepository.Object, _mockCloudProviderRepository.Object);
            _videoUploadForegroundService.Setup(x => x.WaitForNextTick());
        }

        [Fact]
        public async Task UploadVideo_NoVideosAwaitingUpload()
        {
            _mockVideoRepository.Setup(x => x.GetAllVideosAsync()).Returns(Task.FromResult(new List<Videos>()));

            await _videoUploadForegroundService.Object.DoWorkAsync();

            _mockVideoRepository.VerifyAll();
            _mockFileSystemService.VerifyAll();
            _mockCloudProviderRepository.VerifyAll();
            _videoUploadForegroundService.Verify(x => x.WaitForNextTick(), Times.Once);
        }

        [Fact]
        public async Task UploadVideo_SingleFinishedVideoUploaded()
        {
            var fileId = 1;
            var fileName = "TestVid";
            var totalBytes = 100;

            _mockVideoRepository.SetupSequence(x => x.GetAllVideosAsync())
                .Returns(Task.FromResult(new List<Videos>()
            {
                new Videos{ Id = fileId, VideoName = fileName, BytesOffloaded = 0, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() }
            })).Returns(Task.FromResult(new List<Videos>()
            {
                new Videos{ Id = fileId, VideoName = fileName, BytesOffloaded = totalBytes, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() }
            }));

            _mockVideoRepository.Setup(x => x.UpdateBytesOffLoadedOfVideo(fileId, totalBytes)).Returns(Task.CompletedTask);

            var byteArray = Enumerable.Repeat<byte>(9, 100).ToArray();
            _mockFileSystemService.Setup(x => x.GetFileBytesFromCacheDirectory(fileName, 0)).Returns(byteArray);

            _mockCloudProviderRepository.Setup(x => x.AppendContentToCloud(byteArray, fileName)).Returns(Task.CompletedTask);

            await _videoUploadForegroundService.Object.DoWorkAsync();

            _mockVideoRepository.VerifyAll();
            _mockFileSystemService.VerifyAll();
            _mockCloudProviderRepository.VerifyAll();
            _videoUploadForegroundService.Verify(x => x.WaitForNextTick(), Times.Exactly(2));
        }

        [Fact]
        public async Task UploadVideo_MultiFinishedVideoUploaded()
        {
            var firstFileId = 1;
            var secondFileId = 2;
            var firstFileName = "TestVid1";
            var secondFileName = "TestVid2";
            var totalBytes = 100;

            _mockVideoRepository.SetupSequence(x => x.GetAllVideosAsync())
                .Returns(Task.FromResult(new List<Videos>()
            {
                new Videos{ Id = firstFileId, VideoName = firstFileName, BytesOffloaded = 0, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() },
                new Videos{ Id = secondFileId, VideoName = secondFileName, BytesOffloaded = 0, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() }

            })).Returns(Task.FromResult(new List<Videos>()
            {
                 new Videos{ Id = firstFileId, VideoName = firstFileName, BytesOffloaded = totalBytes, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() },
                new Videos{ Id = secondFileId, VideoName = secondFileName, BytesOffloaded = totalBytes, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() }
            }));

            _mockVideoRepository.Setup(x => x.UpdateBytesOffLoadedOfVideo(firstFileId, totalBytes)).Returns(Task.CompletedTask);
            _mockVideoRepository.Setup(x => x.UpdateBytesOffLoadedOfVideo(secondFileId, totalBytes)).Returns(Task.CompletedTask);

            var byteArray = Enumerable.Repeat<byte>(9, 100).ToArray();
            _mockFileSystemService.Setup(x => x.GetFileBytesFromCacheDirectory(firstFileName, 0)).Returns(byteArray);
            _mockFileSystemService.Setup(x => x.GetFileBytesFromCacheDirectory(secondFileName, 0)).Returns(byteArray);

            _mockCloudProviderRepository.Setup(x => x.AppendContentToCloud(byteArray, firstFileName)).Returns(Task.CompletedTask);
            _mockCloudProviderRepository.Setup(x => x.AppendContentToCloud(byteArray, secondFileName)).Returns(Task.CompletedTask);

            await _videoUploadForegroundService.Object.DoWorkAsync();

            _mockVideoRepository.VerifyAll();
            _mockFileSystemService.VerifyAll();
            _mockCloudProviderRepository.VerifyAll();
            _videoUploadForegroundService.Verify(x => x.WaitForNextTick(), Times.Exactly(2));
        }

        [Fact]
        public async Task UploadVideo_SingleInProcessVideoThatFinishes()
        {
            var fileId = 1;
            var fileName = "TestVid";
            var partialBytes = 100;
            var totalBytes = 200;

            _mockVideoRepository.SetupSequence(x => x.GetAllVideosAsync())
                .Returns(Task.FromResult(new List<Videos>()
            {
                new Videos{ Id = fileId, VideoName = fileName, BytesOffloaded = 0, TotalBytes = partialBytes, VideoState = VideoStates.Recording.ToString() }
            })).Returns(Task.FromResult(new List<Videos>()
            {
                new Videos{ Id = fileId, VideoName = fileName, BytesOffloaded = partialBytes, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() }
            })).Returns(Task.FromResult(new List<Videos>()
            {
                new Videos{ Id = fileId, VideoName = fileName, BytesOffloaded = totalBytes, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() }
            }));

            _mockVideoRepository.Setup(x => x.UpdateBytesOffLoadedOfVideo(fileId, It.IsAny<long>())).Returns(Task.CompletedTask);

            var byteArray = Enumerable.Repeat<byte>(9, 100).ToArray();
            _mockFileSystemService.Setup(x => x.GetFileBytesFromCacheDirectory(fileName, 0)).Returns(byteArray);
            _mockFileSystemService.Setup(x => x.GetFileBytesFromCacheDirectory(fileName, 100)).Returns(byteArray);

            _mockCloudProviderRepository.Setup(x => x.AppendContentToCloud(It.IsAny<byte[]>(), fileName)).Returns(Task.CompletedTask);

            await _videoUploadForegroundService.Object.DoWorkAsync();

            _mockVideoRepository.VerifyAll();
            _mockFileSystemService.VerifyAll();
            _mockCloudProviderRepository.VerifyAll();
            _videoUploadForegroundService.Verify(x => x.WaitForNextTick(), Times.Exactly(3));
        }

        [Fact]
        public async Task UploadVideo_DoubleInProcessVideoThatFinishes()
        {
            var firstFileId = 1;
            var secondFileId = 2;
            var firstFileName = "TestVid1";
            var secondFileName = "TestVid2";
            var partialBytes = 100;
            var totalBytes = 200;

            _mockVideoRepository.SetupSequence(x => x.GetAllVideosAsync())
                .Returns(Task.FromResult(new List<Videos>()
            {
                new Videos{ Id = firstFileId, VideoName = firstFileName, BytesOffloaded = 0, TotalBytes = partialBytes, VideoState = VideoStates.Finished.ToString() },
                new Videos{ Id = secondFileId, VideoName = secondFileName, BytesOffloaded = 0, TotalBytes = partialBytes, VideoState = VideoStates.Finished.ToString() }

            })).Returns(Task.FromResult(new List<Videos>()
            {
                 new Videos{ Id = firstFileId, VideoName = firstFileName, BytesOffloaded = partialBytes, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() },
                new Videos{ Id = secondFileId, VideoName = secondFileName, BytesOffloaded = partialBytes, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() }
            })).Returns(Task.FromResult(new List<Videos>()
            {
                 new Videos{ Id = firstFileId, VideoName = firstFileName, BytesOffloaded = totalBytes, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() },
                new Videos{ Id = secondFileId, VideoName = secondFileName, BytesOffloaded = totalBytes, TotalBytes = totalBytes, VideoState = VideoStates.Finished.ToString() }
            }));

            _mockVideoRepository.Setup(x => x.UpdateBytesOffLoadedOfVideo(firstFileId, partialBytes)).Returns(Task.CompletedTask);
            _mockVideoRepository.Setup(x => x.UpdateBytesOffLoadedOfVideo(secondFileId, partialBytes)).Returns(Task.CompletedTask);

            _mockVideoRepository.Setup(x => x.UpdateBytesOffLoadedOfVideo(firstFileId, totalBytes)).Returns(Task.CompletedTask);
            _mockVideoRepository.Setup(x => x.UpdateBytesOffLoadedOfVideo(secondFileId, totalBytes)).Returns(Task.CompletedTask);

            var byteArray = Enumerable.Repeat<byte>(9, 100).ToArray();
            _mockFileSystemService.Setup(x => x.GetFileBytesFromCacheDirectory(firstFileName, 0)).Returns(byteArray);
            _mockFileSystemService.Setup(x => x.GetFileBytesFromCacheDirectory(secondFileName, 0)).Returns(byteArray);

            _mockFileSystemService.Setup(x => x.GetFileBytesFromCacheDirectory(firstFileName, partialBytes)).Returns(byteArray);
            _mockFileSystemService.Setup(x => x.GetFileBytesFromCacheDirectory(secondFileName, partialBytes)).Returns(byteArray);

            _mockCloudProviderRepository.Setup(x => x.AppendContentToCloud(byteArray, firstFileName)).Returns(Task.CompletedTask);
            _mockCloudProviderRepository.Setup(x => x.AppendContentToCloud(byteArray, secondFileName)).Returns(Task.CompletedTask);

            await _videoUploadForegroundService.Object.DoWorkAsync();

            _mockVideoRepository.VerifyAll();
            _mockFileSystemService.VerifyAll();
            _mockCloudProviderRepository.VerifyAll();
            _videoUploadForegroundService.Verify(x => x.WaitForNextTick(), Times.Exactly(3));
        }
    }
}

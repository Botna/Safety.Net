using FluentAssertions;
using Moq;
using WatchMe.Camera;
using WatchMe.Persistance.CloudProviders;
using WatchMe.Persistance.Sqlite;
using WatchMe.Persistance.Sqlite.Tables;
using WatchMe.Repository;
using WatchMe.Services;
using WatchMe.Services.ForegroundServices;

namespace WatchMe.UnitTests.Services
{
    public class OrchestrationServiceTests
    {
        [Fact]
        public async void InitiateRecording_HappyPath()
        {
            var messageRequest = string.Empty;

            var notificationServiceMock = new Mock<INotificationService>();
            notificationServiceMock.Setup(x => x.SendTextToConfiguredContact(It.IsAny<string>())).Callback<string>(r => messageRequest = r).Returns(Task.CompletedTask);
            var fileSystemServiceMock = new Mock<IFileSystemService>();
            fileSystemServiceMock.Setup(x => x.BuildCacheFileDirectory(It.IsAny<string>())).Returns((string input) => { return input; });
            fileSystemServiceMock.Setup(x => x.MoveVideoToGallery(It.IsAny<string>())).Returns(Task.FromResult(new byte[5]));

            var cloudProviderServiceMock = new Mock<ICloudProviderService>();
            cloudProviderServiceMock.Setup(x => x.UploadContentToCloud(It.IsAny<FileStream>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            var databaseInitializerMock = new Mock<IDatabaseInitializer>();
            databaseInitializerMock.Setup(x => x.Init());

            var videosRepositoryMock = new Mock<IVideosRepository>();
            videosRepositoryMock.Setup(x => x.GetVideosByVideoName(It.IsAny<string>())).Returns(Task.FromResult(new Videos()));
            videosRepositoryMock.Setup(x => x.InsertVideosAsync(It.IsAny<Videos>())).Returns(Task.FromResult(1));

            var cameraWrapperMock = new Mock<ICameraWrapper>();
            cameraWrapperMock.Setup(x => x.GetAvailableResolutions(It.IsAny<CameraPosition>())).Returns(new List<Size>() { new Size(1920, 1080) });

            var serviceDispatcherMock = new Mock<IForegroundServiceDispatcher>();
            serviceDispatcherMock.Setup(x => x.StartVUFS());

            var service = new OrchestrationService(cloudProviderServiceMock.Object, fileSystemServiceMock.Object, notificationServiceMock.Object, databaseInitializerMock.Object, videosRepositoryMock.Object, cameraWrapperMock.Object, serviceDispatcherMock.Object);
            service.Initialize(new CameraView(), new CameraView());

            await service.InitiateRecordingProcedure();
            messageRequest.Should().Contain("just started a WatchMe Routine");

            await service.StopRecordingProcedure();

            notificationServiceMock.VerifyAll();
            fileSystemServiceMock.VerifyAll();
            cloudProviderServiceMock.VerifyAll();
            databaseInitializerMock.VerifyAll();
            videosRepositoryMock.VerifyAll();
            cameraWrapperMock.VerifyAll();
            serviceDispatcherMock.VerifyAll();
        }
    }
}

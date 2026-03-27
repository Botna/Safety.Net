//using WatchMe.Camera;

//namespace WatchMe.Services
//{
//    public interface ICameraWrapper
//    {
//        public void Initialize(CameraView front, CameraView back);
//        public List<Size>? GetAvailableResolutions(CameraPosition cameraPosition);

//        public Task<CameraResult> StartRecordingAsync(CameraPosition cameraPosition, string? filename, Size size);

//        public Task<CameraResult> StopCameraAsync(CameraPosition cameraPosition);
//    }
//    public class CameraWrapper : ICameraWrapper
//    {
//        private CameraView _front;
//        private CameraView _back;

//        public void Initialize(CameraView front, CameraView back)
//        {
//            _front = front;
//            _back = back;
//        }

//        public List<Size>? GetAvailableResolutions(CameraPosition cameraPosition)
//        {
//            var camera = GetCameraByPosition(cameraPosition);
//            return camera.Camera.AvailableResolutions;
//        }

//        public async Task<CameraResult> StartRecordingAsync(CameraPosition cameraPosition, string? filename, Size size)
//        {
//            var camera = GetCameraByPosition(cameraPosition);
//            return await camera.StartRecordingAsync(filename, size);
//        }

//        public async Task<CameraResult> StopCameraAsync(CameraPosition cameraPosition)
//        {
//            var camera = GetCameraByPosition(cameraPosition);
//            return await camera.StopRecordingAsync();
//        }

//        private CameraView GetCameraByPosition(CameraPosition cameraPosition)
//        {
//            CameraView camera = null;

//            switch (cameraPosition)
//            {
//                case CameraPosition.Front:
//                    camera = _front;
//                    break;
//                case CameraPosition.Back:
//                    camera = _back;
//                    break;
//                default:
//                    throw new Exception("Incorrect Camera Poisition requested");
//            }
//            return camera;
//        }


//    }
//}

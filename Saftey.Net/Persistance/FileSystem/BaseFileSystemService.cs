using WatchMe.Repository;

namespace WatchMe.Persistance.Implementations
{
    public abstract class BaseFileSystemService : IFileSystemService
    {
        private const long HEADER_BYTES_SIZE = 16384;

        public abstract Task MoveVideoToGallery(MemoryStream memStream, string fileName);

        public byte[]? GetFileBytesFromCacheDirectory(string fileName, long byteOffset, long maxBytes = 8388608)
        {
            var filePath = BuildCacheFileDirectory(fileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var currentMax = fileStream.Length;
                byte[] buffer = new byte[4096];
                long bytesRemaining = maxBytes;
                using (MemoryStream ms = new MemoryStream())
                {
                    int bytesRead;


                    fileStream.Seek(byteOffset, SeekOrigin.Current);

                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        bytesRemaining -= bytesRead;
                        ms.Write(buffer, 0, bytesRead);
                        if (bytesRemaining == 0)
                        {
                            break;

                        }
                    }
                    return ms.ToArray();
                }
            }
        }

        public byte[]? GetInitialBytesFromFile(string fileName)
        {
            var filePath = BuildCacheFileDirectory(fileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var currentMax = fileStream.Length;
                byte[] buffer = new byte[HEADER_BYTES_SIZE];

                using (MemoryStream ms = new MemoryStream())
                {
                    int bytesRead;
                    fileStream.Seek(0, SeekOrigin.Current);

                    bytesRead = fileStream.Read(buffer, 0, buffer.Length);
                    ms.Write(buffer, 0, bytesRead);

                    return ms.ToArray();
                }
            }
        }

        public string BuildCacheFileDirectory(string fileName) =>
            Path.Combine(FileSystem.Current.CacheDirectory, fileName);
    }
}

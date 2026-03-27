using WatchMe.Repository;

namespace WatchMe.Persistance.Implementations
{
    public abstract class BaseFileSystemService : IFileSystemService
    {
        //public FileStream GetFileStreamOfFile(string fileName)
        //{
        //    return new FileStream(BuildCacheFileDirectory(fileName), FileMode.Open);
        //}

        public abstract Task MoveVideoToGallery(MemoryStream memStream, string fileName);

        public byte[]? GetFileBytesFromCacheDirectory(string fileName, long byteOffset)
        {
            var filePath = BuildCacheFileDirectory(fileName);
            using (FileStream fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var currentMax = fileStream.Length;
                byte[] buffer = new byte[4096];

                using (MemoryStream ms = new MemoryStream())
                {
                    int bytesRead;
                    fileStream.Seek(byteOffset, SeekOrigin.Current);
                    while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        ms.Write(buffer, 0, bytesRead);
                    }
                    return ms.ToArray();
                }
            }
        }

        public string BuildCacheFileDirectory(string fileName) =>
            Path.Combine(FileSystem.Current.CacheDirectory, fileName);
    }
}

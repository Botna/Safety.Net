namespace WatchMe.Repository
{
    public interface IFileSystemService
    {
        Task MoveVideoToGallery(MemoryStream memStream, string fileName);
        //FileStream GetFileStreamOfFile(string filePath);
        //Task<byte[]?> GetAllFileBytesFromCacheDirectory(string fileName);
        //string BuildCacheFileDirectory(string fileName);
        byte[]? GetFileBytesFromCacheDirectory(string fileName, long byteOffset);

    }
}


#if IOS
using WatchMe.Repository;

namespace WatchMe.Persistance.Implementations
{

    internal class IOSFileSystemService : BaseFileSystemService
    {
        public override FileStream GetFileStreamOfFile(string filename)
        {
            throw new NotImplementedException();
        }

        public override bool SaveVideoToFileSystem(byte[] videoBytes, string fileName)
        {
            throw new NotImplementedException();
        }
    }
}
#endif
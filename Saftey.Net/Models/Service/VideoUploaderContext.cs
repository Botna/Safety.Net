using WatchMe.Persistance.Sqlite;
using WatchMe.Repository;

namespace WatchMe.Models.Service
{
    public class VideoUploaderContext
    {
        public IVideoChunksRepository VideoChunksRepository { get; set; }
        public IVideosRepository VideosRepository { get; set; }
        public IFileSystemService FileSystemService { get; set; }

    }
}

using SQLite;

namespace WatchMe.Persistance.Sqlite.Tables
{
    public class VideoChunks
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int VideoId { get; set; }
        public int VideoChunkNumber { get; set; }
        public long StartByte { get; set; }
        public long EndByte { get; set; }
    }
}

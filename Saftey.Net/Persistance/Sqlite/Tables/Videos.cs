using SQLite;

namespace WatchMe.Persistance.Sqlite.Tables
{
    public class Videos
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public string VideoName { get; set; }
        public string VideoState { get; set; }
        public long BytesOffloaded { get; set; }
        public long TotalBytes { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}

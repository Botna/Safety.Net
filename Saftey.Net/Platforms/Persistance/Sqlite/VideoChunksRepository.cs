using SQLite;
using WatchMe.Persistance.Sqlite.Tables;

namespace WatchMe.Persistance.Sqlite
{
    public interface IVideoChunksRepository
    {
        public Task<List<VideoChunks>> GetVideoChunksByVideoIdsAsync(IEnumerable<int> videoIds);
        public Task<int> InsertVideoChunksAsync(params VideoChunks[] items);

        public Task<int> UpdateVideoChunksAsync(params VideoChunks[] items);

        public Task<int> DeleteVideoChunksAsync(params VideoChunks[] items);
    }

    public class VideoChunksRepository : IVideoChunksRepository
    {
        public Task<int> DeleteVideoChunksAsync(params VideoChunks[] items)
        {
            throw new NotImplementedException();
        }

        public async Task<List<VideoChunks>> GetVideoChunksByVideoIdsAsync(IEnumerable<int> videoIds)
        {
            var database = GetConnection();

            //TODO, does this work quickly at all?  Do we care
            return await database.Table<VideoChunks>().Where(x => videoIds.Contains(x.VideoId)).ToListAsync();
        }

        public Task<int> InsertVideoChunksAsync(params VideoChunks[] items)
        {
            throw new NotImplementedException();
        }

        public Task<int> UpdateVideoChunksAsync(params VideoChunks[] items)
        {
            throw new NotImplementedException();
        }

        public virtual ISQLiteAsyncConnection GetConnection() =>
            new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
    }
}

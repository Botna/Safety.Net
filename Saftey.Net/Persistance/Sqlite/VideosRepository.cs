using SQLite;
using WatchMe.Persistance.Sqlite.Tables;

namespace WatchMe.Persistance.Sqlite
{
    public interface IVideosRepository
    {
        public Task<List<Videos>> GetAllVideosAsync();
        public Task<Videos> GetVideosByVideoName(string videoName);
        public Task<int> InsertVideosAsync(params Videos[] items);
        public Task<int> UpdateVideosAsync(ISQLiteAsyncConnection connection, params Videos[] items);

        public Task UpdateBytesOffLoadedOfVideo(int id, long bytesOffloaded);
        public Task UpdateTotalBytesOfVideo(int id, long totalBytes);

        public Task UpdateStateOfVideo(int id, VideoStates state);
        public Task<int> DeleteVideosAsync(params Videos[] items);
    }

    public class VideosRepository : IVideosRepository
    {
        public async Task<List<Videos>> GetAllVideosAsync()
        {
            var database = GetConnection();
            var results = await database.Table<Videos>().ToListAsync();
            await database.CloseAsync();
            return results;
        }



        public async Task<Videos> GetVideosByVideoName(string videoName)
        {
            var database = GetConnection();
            var results = await database.Table<Videos>().Where(x => x.VideoName == videoName).FirstOrDefaultAsync();
            await database.CloseAsync();
            return results;
        }

        public async Task<int> InsertVideosAsync(params Videos[] items)
        {
            var database = GetConnection();
            var count = 0;
            foreach (var item in items)
            {
                await database.InsertAsync(item);
                count++;
            }
            await database.CloseAsync();
            return count;
        }

        public async Task<int> UpdateVideosAsync(ISQLiteAsyncConnection connection, params Videos[] items)
        {
            var database = connection ?? GetConnection();
            var count = 0;
            foreach (var item in items)
            {
                await database.UpdateAsync(item);
                count++;
            }
            await database.CloseAsync();
            return count;
        }

        public async Task<int> DeleteVideosAsync(params Videos[] items)
        {
            var database = GetConnection();
            var count = 0;
            foreach (var item in items)
            {
                await database.DeleteAsync(item);
                count++;
            }
            await database.CloseAsync();
            return count;
        }

        public virtual ISQLiteAsyncConnection GetConnection() =>
             new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

        public async Task UpdateBytesOffLoadedOfVideo(int id, long bytesOffloaded)
        {
            var database = GetConnection();
            var record = await database.Table<Videos>().Where(x => x.Id == id).FirstOrDefaultAsync();
            record.BytesOffloaded = bytesOffloaded;
            await UpdateVideosAsync(database, record);
            await database.CloseAsync();
        }

        public async Task UpdateTotalBytesOfVideo(int id, long totalBytes)
        {
            var database = GetConnection();
            var record = await database.Table<Videos>().Where(x => x.Id == id).FirstOrDefaultAsync();
            record.TotalBytes = totalBytes;
            await UpdateVideosAsync(database, record);
            await database.CloseAsync();
        }

        public async Task UpdateStateOfVideo(int id, VideoStates state)
        {
            var database = GetConnection();
            var record = await database.Table<Videos>().Where(x => x.Id == id).FirstOrDefaultAsync();
            record.VideoState = state.ToString();
            await UpdateVideosAsync(database, record);
            await database.CloseAsync();
        }
    }
}

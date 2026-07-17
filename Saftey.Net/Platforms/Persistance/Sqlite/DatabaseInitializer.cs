using SQLite;
using WatchMe.Persistance.Sqlite.Tables;

namespace WatchMe.Persistance.Sqlite
{
    public interface IDatabaseInitializer
    {
        Task Init();
    }
    public class DatabaseInitializer : IDatabaseInitializer
    {
        public async Task Init()
        {
            var database = new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);

            var tableTypes = new List<Type>()
            {
                typeof(VideoChunks),
                typeof(Videos)
            };

            var result = await database.CreateTablesAsync(CreateFlags.ImplicitIndex, tableTypes.ToArray());
        }
    }
}

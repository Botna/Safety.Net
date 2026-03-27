namespace WatchMe.Services.ForegroundServices
{
    public interface IForegroundService
    {
        Task DoWorkAsync();

        void StopService();
    }
}

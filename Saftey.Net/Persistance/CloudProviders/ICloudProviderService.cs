namespace WatchMe.Persistance.CloudProviders
{
    public interface ICloudProviderService
    {
        Task UploadContentToCloud(Stream fileStream, string contentName);
        Task AppendContentToCloud(byte[] bytes, string contentName);
        Task<string> GetAzureConnectionString();
        Task SetAzureConnectionString(string connstr);
    }
}

//using Azure.Storage.Blobs;
//using Azure.Storage.Blobs.Specialized;
//using WatchMe.Helpers;

//namespace Safety.Net.Persistance.CloudProviders
//{
//    public class AzureService : ICloudProviderService
//    {
//        private const string AZURESTORAGECONTAINERCONNECTIONSTRINGKEY = "azure_storagecontainer_connectionstring";
//        public async Task UploadContentToCloud(Stream fileStream, string contentName)
//        {
//            //Right now, we only have Azure configured.

//            string storageContainerConnectionString = await SecureStorage.Default.GetAsync(AZURESTORAGECONTAINERCONNECTIONSTRINGKEY);

//            if (storageContainerConnectionString == null)
//            {
//                throw new Exception($"unable to gather {AZURESTORAGECONTAINERCONNECTIONSTRINGKEY} from secure storage");
//            }
//            try
//            {
//                var blobServiceClient = new BlobServiceClient(storageContainerConnectionString);

//                string containerName = "watchmefileuploadcontainer";

//                var containerClient = blobServiceClient.GetBlobContainerClient(containerName);


//                BlobClient blobClient = containerClient.GetBlobClient(contentName);
//                var result = await blobClient.UploadAsync(fileStream, true);
//            }
//            catch (Exception)
//            {
//                ToastHelper.CreateToast("Issue uploading to SC");
//            }
//        }

//        public async Task AppendContentToCloud(byte[] bytes, string contentName)
//        {
//            try
//            {
//                string storageContainerConnectionString = await GetAzureConnectionString();

//                var containerClient = new BlobContainerClient(storageContainerConnectionString, "watchme");
//                await containerClient.CreateIfNotExistsAsync();

//                var appendBlobClient = containerClient.GetAppendBlobClient(contentName);
//                await appendBlobClient.CreateIfNotExistsAsync();

//                int maxBlockSize = appendBlobClient.AppendBlobMaxAppendBlockBytes;
//                long bytesLeft = bytes.Length;
//                var count = 0;
//                while (bytesLeft > 0)
//                {
//                    int blockSize = (int)Math.Min(bytesLeft, maxBlockSize);
//                    var buffer = bytes.Skip(count).Take(blockSize).ToArray();
//                    await using (MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length))
//                    {
//                        await appendBlobClient.AppendBlockAsync(memoryStream);
//                    }
//                    bytesLeft -= buffer.Length;
//                }

//            }
//            catch (Exception)
//            {
//                await ToastHelper.CreateToast("issue uploading to AppendBlob");
//            }
//        }

//        public async Task<string> GetAzureConnectionString() =>
//            await SecureStorage.Default.GetAsync(AZURESTORAGECONTAINERCONNECTIONSTRINGKEY);

//        public async Task SetAzureConnectionString(string connstr) =>
//            await SecureStorage.Default.SetAsync(AZURESTORAGECONTAINERCONNECTIONSTRINGKEY, connstr);
//    }
//}

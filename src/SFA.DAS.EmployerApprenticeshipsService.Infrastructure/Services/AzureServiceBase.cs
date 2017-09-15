using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.EnvironmentInfo;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public abstract class AzureServiceBase<T>
    {
        public abstract string ConfigurationName { get; }
        public abstract ILog Logger { get; set; }
        
        public async Task<T> GetModelFromBlobStorage(string containerName, string blobName)
        {
            using (var blobData = await GetBlobDataFromAzure(containerName, blobName))
            {
                using (var reader = new StreamReader(blobData))
                {
                    var jsonContent = reader.ReadToEnd();

                    return string.IsNullOrEmpty(jsonContent)
                        ? default(T)
                        : JsonConvert.DeserializeObject<T>(jsonContent);
                }
            }
        }
        
        public async Task<MemoryStream> GetBlobDataFromAzure(string blobContainer, string blobName)
        {
            try
            {
                var storageAccount =
                    CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));

                var client = storageAccount.CreateCloudBlobClient();

                var container = client.GetContainerReference(blobContainer);

                var blob = container.GetBlobReference(blobName);

                var stream = new MemoryStream();
                
                await blob.DownloadRangeToStreamAsync(stream, 0, null);
                
                return stream;
                
            }
            catch (StorageException e)
            {
                Logger.Warn(e, "Unable to get blob from azure storage");
            }

            return null;
        }
    }
}
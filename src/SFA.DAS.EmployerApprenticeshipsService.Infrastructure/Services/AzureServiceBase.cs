using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public abstract class AzureServiceBase<T>
    {
        public abstract string ConfigurationName { get; }
        public abstract ILog Logger { get; set; }

        public async Task<T> GetDataFromBlobStorage(string containerName, string blobName)
        {
            using (var blobData = await StreamDataFromBlobStorage(containerName, blobName))
            {
                using (var reader = new StreamReader(blobData))
                {
                    var value = reader.ReadToEnd();

                    return string.IsNullOrEmpty(value)
                        ? default(T)
                        : JsonConvert.DeserializeObject<T>(value);
                }
            }
        }

        public virtual T GetDataFromTableStorage()
        {
            return ConfigurationHelper.GetConfiguration<T>(ConfigurationName);
        }

        public Task<T> GetDataFromTableStorageAsync()
        {
            return ConfigurationHelper.GetConfigurationAsync<T>(ConfigurationName);
        }

        public async Task<MemoryStream> StreamDataFromBlobStorage(string containerName, string blobName)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                var client = storageAccount.CreateCloudBlobClient();
                var container = client.GetContainerReference(containerName);
                var blob = container.GetBlobReference(blobName);
                var stream = new MemoryStream();

                await blob.DownloadRangeToStreamAsync(stream, 0, null);
                
                return stream;
            }
            catch (StorageException ex)
            {
                Logger.Warn(ex, "Unable to get blob from azure storage.");
            }

            return null;
        }
    }
}
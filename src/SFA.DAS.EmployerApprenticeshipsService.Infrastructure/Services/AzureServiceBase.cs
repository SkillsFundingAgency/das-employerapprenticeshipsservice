using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public abstract class AzureServiceBase
    {
        public abstract string ConfigurationName { get; }
        public ILog Logger { get; set; }
        
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
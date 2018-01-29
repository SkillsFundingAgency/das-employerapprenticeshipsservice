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
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public abstract class AzureServiceBase<T>
    {
        public abstract string ConfigurationName { get; }
        public abstract ILog Logger { get; set; }

        public virtual T GetDataFromStorage()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");

            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var configurationRepository = GetDataFromAzure();
            var configurationService = new ConfigurationService(configurationRepository, new ConfigurationOptions(ConfigurationName, environment, "1.0"));
            var config = configurationService.Get<T>();

            return config;
        }

        public async Task<T> GetModelFromBlobStorage(string containerName, string blobName)
        {
            using (var blobData = await GetBlobDataFromAzure(containerName, blobName))
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
        
        public async Task<MemoryStream> GetBlobDataFromAzure(string blobContainer, string blobName)
        {
            try
            {
                var storageAccount = CloudStorageAccount.Parse(CloudConfigurationManager.GetSetting("StorageConnectionString"));
                var client = storageAccount.CreateCloudBlobClient();
                var container = client.GetContainerReference(blobContainer);
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

        protected IConfigurationRepository GetDataFromAzure()
        {
            IConfigurationRepository configurationRepository;

            if (bool.Parse(ConfigurationManager.AppSettings["LocalConfig"]))
            {
                configurationRepository = new FileStorageConfigurationRepository();
            }
            else
            {
                configurationRepository = new AzureTableStorageConfigurationRepository(CloudConfigurationManager.GetSetting("ConfigurationStorageConnectionString"));
            }

            return configurationRepository;
        }
    }
}
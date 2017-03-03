using System;
using System.Configuration;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using NLog;
using SFA.DAS.Configuration;
using SFA.DAS.Configuration.AzureTableStorage;
using SFA.DAS.Configuration.FileStorage;

namespace SFA.DAS.EAS.Infrastructure.Services
{
    public abstract class AzureServiceBase<T>
    {
        public abstract string ConfigurationName { get; }
        public abstract ILogger Logger { get; set; }
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

        public virtual T GetDataFromStorage()
        {
            var environment = Environment.GetEnvironmentVariable("DASENV");
            if (string.IsNullOrEmpty(environment))
            {
                environment = CloudConfigurationManager.GetSetting("EnvironmentName");
            }

            var configurationRepository = GetDataFromAzure();
            var configurationService = new ConfigurationService(
                configurationRepository,
                new ConfigurationOptions(ConfigurationName, environment, "1.0"));

            var config = configurationService.Get<T>();

            return config;
        }

        public async Task<T> GetModelFromBlobStorage<T>(string containerName, string blobName)
        {
            var blobData = await GetBlobDataFromAzure(containerName, blobName);

            if (blobData == null)
            {
                return default(T);
            }

            using (var stream = new MemoryStream(blobData.ToArray()))
            {
                using (var reader = new StreamReader(stream))
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

                using (var stream = new MemoryStream())
                {
                    await blob.DownloadRangeToStreamAsync(stream, 0, null);

                    return stream;
                }
            }
            catch (StorageException e)
            {
                Logger.Warn(e, "Unable to get blob from azure storage");
            }

            return null;
        }
    }
}
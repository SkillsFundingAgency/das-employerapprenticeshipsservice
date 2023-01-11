using System.Configuration;
using System.IO;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using SFA.DAS.AutoConfiguration;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Services;

public abstract class AzureServiceBase<T>
{
    private readonly IAutoConfigurationService _autoConfigurationService;

    protected AzureServiceBase(IAutoConfigurationService autoConfigurationService)
    {
        _autoConfigurationService = autoConfigurationService;
    }

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
        return _autoConfigurationService.Get<T>(ConfigurationName);
    }

    public async Task<MemoryStream> StreamDataFromBlobStorage(string containerName, string blobName)
    {
        try
        {
            var storageAccount = CloudStorageAccount.Parse(ConfigurationManager.AppSettings["StorageConnectionString"]);
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
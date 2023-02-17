using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Newtonsoft.Json;
using SFA.DAS.AutoConfiguration;

namespace SFA.DAS.EmployerAccounts.Services;

public abstract class AzureServiceBase<T, TLogger>
{
    private readonly IAutoConfigurationService _autoConfigurationService;
    private readonly IConfiguration _configuration;

    protected AzureServiceBase(IAutoConfigurationService autoConfigurationService, IConfiguration configuration)
    {
        _autoConfigurationService = autoConfigurationService;
        _configuration = configuration;
    }

    public abstract string ConfigurationName { get; }
    public abstract ILogger<TLogger> Logger { get; set; }

    public async Task<T> GetDataFromBlobStorage(string containerName, string blobName)
    {
        using (var blobData = await StreamDataFromBlobStorage(containerName, blobName))
        {
            using (var reader = new StreamReader(blobData))
            {
                var value = await reader.ReadToEndAsync();

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
            var storageAccount = CloudStorageAccount.Parse(_configuration["StorageConnectionString"]);
            var client = storageAccount.CreateCloudBlobClient();
            var container = client.GetContainerReference(containerName);
            var blob = container.GetBlobReference(blobName);
            var stream = new MemoryStream();

            await blob.DownloadRangeToStreamAsync(stream, 0, null);

            return stream;
        }
        catch (StorageException ex)
        {
            Logger.LogWarning(ex, "Unable to get blob from azure storage.");
        }

        return null;
    }
}
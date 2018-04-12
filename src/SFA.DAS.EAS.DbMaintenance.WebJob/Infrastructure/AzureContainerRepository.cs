using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure.Interfaces;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure
{
	public class AzureContainerRepository : IAzureContainerRepository
	{
		private readonly IWebJobConfiguration _webJobConfiguration;

		public AzureContainerRepository(IWebJobConfiguration webJobConfiguration)
		{
			_webJobConfiguration = webJobConfiguration;
		}

		public Task EnsureQueueExistsAsync(string queueName)
		{
			return CreateQueueAsync(queueName);
		}

		private Task CreateQueueAsync(string queueName)
		{
			var storageAccount = CreateStorageAccountFromConnectionString(_webJobConfiguration.StorageConnectionString);
			var queueClient = storageAccount.CreateCloudQueueClient();
			var queue = queueClient.GetQueueReference(queueName);
			return queue.CreateIfNotExistsAsync();
		}

		private CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
		{
			return CloudStorageAccount.Parse(storageConnectionString);
		}
	}
}
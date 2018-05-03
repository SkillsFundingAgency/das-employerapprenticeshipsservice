using System;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Account.Worker.Infrastructure
{
	public class AzureQueueClient : IAzureQueueClient
	{
		private readonly IWebJobConfiguration _webJobConfiguration;

		private readonly Lazy<CloudQueueClient> _queueClient;

		protected CloudQueueClient QueueClient => _queueClient.Value;

		public AzureQueueClient(IWebJobConfiguration webJobConfiguration)
		{
			_webJobConfiguration = webJobConfiguration;
			_queueClient = new Lazy<CloudQueueClient>(Initialise);
		}

		public Task EnsureQueueExistsAsync(string queueName)
		{
			return CreateQueueAsync(queueName);
		}

		public void QueueMessage<TMessage>(string queueName, TMessage message)
		{
			var queue = QueueClient.GetQueueReference(queueName);
			queue.AddMessage(new CloudQueueMessage(Newtonsoft.Json.JsonConvert.SerializeObject(message)));
		}

		private Task CreateQueueAsync(string queueName)
		{
			var queue = QueueClient.GetQueueReference(queueName);
			return queue.CreateIfNotExistsAsync();
		}

		private CloudStorageAccount CreateStorageAccountFromConnectionString(string storageConnectionString)
		{
			return CloudStorageAccount.Parse(storageConnectionString);
		}

		private CloudQueueClient Initialise()
		{
			var storageAccount = CreateStorageAccountFromConnectionString(_webJobConfiguration.StorageConnectionString);
			var queueClient = storageAccount.CreateCloudQueueClient();
			return queueClient;
		}
	}
}
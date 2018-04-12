using System.Threading.Tasks;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure.Interfaces
{
	/// <summary>
	///		Represents a service for managing the existance of Azure storage account contains such as queues.
	/// </summary>
	public interface IAzureContainerRepository
	{
		Task EnsureQueueExistsAsync(string queueName);

		void QueueMessage<TMessage>(string queueName, TMessage message);
	}
}
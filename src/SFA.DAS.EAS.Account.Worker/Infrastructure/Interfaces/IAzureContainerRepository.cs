using System.Threading.Tasks;

namespace SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces
{
	/// <summary>
	///		Represents a service for managing the existence of Azure queues.
	/// </summary>
	public interface IAzureQueueClient
	{
		Task EnsureQueueExistsAsync(string queueName);

		void QueueMessage<TMessage>(string queueName, TMessage message);
	}
}
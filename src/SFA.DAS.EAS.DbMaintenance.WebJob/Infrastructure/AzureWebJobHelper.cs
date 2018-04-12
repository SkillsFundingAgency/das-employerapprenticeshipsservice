using SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure
{
	public class AzureWebJobHelper : IAzureWebJobHelper
	{
		private readonly ITriggeredJobRepository _triggeredJobRepository;
		private readonly IAzureContainerRepository _azureContainerRepository;
		private readonly ILog _logger;

		public AzureWebJobHelper(ITriggeredJobRepository triggeredJobRepository, IAzureContainerRepository azureContainerRepository, ILog logger)
		{
			_triggeredJobRepository = triggeredJobRepository;
			_azureContainerRepository = azureContainerRepository;
			_logger = logger;
		}

		public void EnsureAllQueuesForTriggeredJobs()
		{
			foreach (var triggeredJob in _triggeredJobRepository.GetQueueuTriggeredJobs())
			{
				_azureContainerRepository.EnsureQueueExistsAsync(triggeredJob.Trigger.QueueName);	
			}
		}
	}
}
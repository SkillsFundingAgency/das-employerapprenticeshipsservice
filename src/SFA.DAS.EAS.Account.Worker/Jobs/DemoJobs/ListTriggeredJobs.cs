using System.Text;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.Jobs.DemoJobs
{
	public class ListTriggeredJobs : IJob
	{
		private readonly ILog _logger;
		private readonly ITriggeredJobRepository _triggeredJobRepository;

		public ListTriggeredJobs(ILog logger, ITriggeredJobRepository triggeredJobRepository)
		{
			_logger = logger;
			_triggeredJobRepository = triggeredJobRepository;
		}

		public Task Run()
		{
			var sb = new StringBuilder();
			int i = 0;

			foreach (var job in _triggeredJobRepository.GetQueuedTriggeredJobs())
			{
				sb.AppendLine($"{i++}: {job.ContainingClass.FullName}.{job.InvokedMethod.Name} invoked by {job.Trigger} on queue {job.Trigger.QueueName}");
			}

			_logger.Info(sb.ToString());
			return Task.CompletedTask;
		}
	}
}
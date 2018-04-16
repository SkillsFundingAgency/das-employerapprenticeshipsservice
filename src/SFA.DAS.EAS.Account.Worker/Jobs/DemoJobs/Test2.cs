using System.Threading.Tasks;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.Jobs.DemoJobs
{
	public class ListRecentActivity : IJob
	{
		private readonly ILog _logger;
		public ListRecentActivity(ILog logger)
		{
			_logger = logger;
		}

		public Task Run()
		{
			return Task.CompletedTask;
		}
	}
}
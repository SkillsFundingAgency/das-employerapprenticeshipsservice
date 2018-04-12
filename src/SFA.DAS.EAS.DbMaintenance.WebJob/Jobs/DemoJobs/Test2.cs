using System.IO;
using System.Threading.Tasks;
using SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Jobs.DemoJobs
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
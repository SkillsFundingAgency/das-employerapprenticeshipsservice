using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.EAS.Account.Worker.Infrastructure;

namespace SFA.DAS.EAS.Account.Worker.Jobs.DemoJobs
{
	public class TickTockJob : IJob
	{
		private readonly TraceWriter _logger;
		public TickTockJob(TraceWriter logger)
		{
			_logger = logger;
		}

		public Task Run()
		{
			_logger.Info("Tick-Tock!!");
			return Task.CompletedTask;
		}
	}
}
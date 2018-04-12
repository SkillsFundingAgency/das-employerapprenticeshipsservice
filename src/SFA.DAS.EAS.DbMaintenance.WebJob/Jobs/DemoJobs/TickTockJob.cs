﻿using System.IO;
using System.Threading.Tasks;
using SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Jobs.DemoJobs
{
	public class TickTockJob : IJob
	{
		private readonly TextWriter _logger;
		public TickTockJob(TextWriter logger)
		{
			_logger = logger;
		}

		public Task Run()
		{
			JobLogger.WriteLine(_logger, "[[Tick-Tock]]");
			return Task.CompletedTask;
		}
	}
}
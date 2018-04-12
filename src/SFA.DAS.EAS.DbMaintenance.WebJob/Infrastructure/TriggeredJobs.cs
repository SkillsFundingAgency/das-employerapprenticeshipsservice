using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using SFA.DAS.EAS.DbMaintenance.WebJob.Jobs;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure
{
	public class TriggeredJobs
	{
		public void ProcessQueueMessage([QueueTrigger(Constants.AzureQueueNames.AdHocJobQueue)] AdHocJobParams jobParams, TextWriter logger)
		{
			// find implementation of IJob named jobParams.JobName
			JobLogger.WriteLine(logger, $"Received request to process job type {jobParams.JobName}");
			var jobtype = GetAllPossibleJobs()
				.FirstOrDefault(jobType => string.Equals(jobType.Name, jobParams.JobName, StringComparison.InvariantCultureIgnoreCase));

			if (jobtype != null)
			{
				RunJob(jobtype, logger);
			}
		}

		//public void TickTockJob([TimerTrigger("0 */1 * * * *")]
		//	AdHocJobParams jobParams, TextWriter logger)
		//{
		//	RunJob(typeof(TickTockJob), logger);
		//}

		private void RunJob(Type jobtype, TextWriter logger)
		{
			var thisContainer = ServiceLocator.CreateChildContainer();
			ServiceLocator.Register<TextWriter>(thisContainer, logger);
			IJob job;
			logger.WriteLine($"found job type {jobtype.FullName}... resolving from IoC...");
			try
			{
				job = (IJob)thisContainer.GetInstance(jobtype);
				JobLogger.WriteLine(logger, $"Obtained instance of type {jobtype.FullName} from IoC");
			}
			catch (Exception e)
			{
				JobLogger.WriteLine(logger, $"Failed to fetch instance of type {jobtype.FullName} from IoC - error: {e.GetType()} - {e.Message}");
				throw;
			}

			job.Run().ContinueWith(task =>
			{
				JobLogger.WriteLine(logger, $"Job has ended. Canceled?:{task.IsCanceled} Faulted?:{task.IsFaulted}");
			});
		}

		private IEnumerable<Type> GetAllPossibleJobs()
		{
			return Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IJob).IsAssignableFrom(t));
		}
	}
}
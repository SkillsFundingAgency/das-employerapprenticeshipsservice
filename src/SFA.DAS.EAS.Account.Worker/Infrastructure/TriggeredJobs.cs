using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces;
using SFA.DAS.EAS.Account.Worker.Jobs;

namespace SFA.DAS.EAS.Account.Worker.Infrastructure
{
	public class TriggeredJobs
	{
		public void ProcessQueueMessage([QueueTrigger(Constants.AzureQueueNames.AdHocJobQueue)] AdHocJobParams jobParams, TraceWriter logger)
		{
			// find implementation of IJob named jobParams.JobName
			logger.Info($"Received request to process job type {jobParams.JobName}");
			var jobtype = GetAllPossibleJobs()
				.FirstOrDefault(jobType => string.Equals(jobType.Name, jobParams.JobName, StringComparison.InvariantCultureIgnoreCase));

			if (jobtype != null)
			{
				RunJob(jobtype, logger);
			}
		}

		//                                    {second} {minute} {hour} {day} {month} {day-of-week}
		public void PaymentCheck([TimerTrigger("0 30 5 1 * *")] TimerInfo timer, TraceWriter logger)
		{
			ScheduleJob<PaymentIntegrityCheckerJob>();	
		}

		private void RunJob(Type jobtype, TraceWriter logger)
		{
		    using (var thisContainer = ServiceLocator.CreateNestedContainer())
		    {
		        ServiceLocator.Register<TraceWriter>(thisContainer, logger);
		        IJob job;
		        logger.Info($"found job type {jobtype.FullName}");
		        try
		        {
		            job = (IJob) thisContainer.GetInstance(jobtype);
		            logger.Verbose($"Obtained instance of type {jobtype.FullName} from IoC");
		        }
		        catch (Exception e)
		        {
		            logger.Error($"Failed to fetch instance of type {jobtype.FullName} from IoC - error: {e.GetType()} - {e.Message}");
		            throw;
		        }

		        job.Run().ContinueWith(task =>
		        {
		            logger.Info($"Job has ended. Canceled?:{task.IsCanceled} Faulted?:{task.IsFaulted}");
		        });
		    }
		}

		private IEnumerable<Type> GetAllPossibleJobs()
		{
			return Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IJob).IsAssignableFrom(t));
		}

		private void ScheduleJob<TJob>() where TJob : IJob
		{
			var azureRepo = ServiceLocator.Get<IAzureQueueClient>();
			azureRepo.QueueMessage(Constants.AzureQueueNames.AdHocJobQueue, new AdHocJobParams {JobName = typeof(TJob).Name });
		}
	}
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using NLog.Internal;
using SFA.DAS.EAS.DbMaintenance.WebJob.DependencyResolution;
using SFA.DAS.EAS.DbMaintenance.WebJob.Jobs;
using SFA.DAS.EAS.DbMaintenance.WebJob.Jobs.GenerateAgreements;
using SFA.DAS.NLog.Logger;
using StructureMap;

namespace SFA.DAS.EAS.DbMaintenance.WebJob
{
    public class Program
    {
        public static void Main()
        {
            var container = IoC.Initialize();
            ServiceLocator.Initialise(container);

			var azureStorageAccount = new ConfigurationManager().AppSettings["SFAStorageAccount"];
			JobHostConfiguration config = new JobHostConfiguration
			{
				DashboardConnectionString = azureStorageAccount,
				StorageConnectionString = azureStorageAccount
			};

	        JobHost host = new JobHost(config);
	        host.RunAndBlock();
		}
	}

	public static class ServiceLocator
	{
		private static IContainer _container;

		public static void Initialise(IContainer container)
		{
			_container = container;
		}

		public static T Get<T>()
		{
			return _container.GetInstance<T>();
		}

		public static T Get<T>(Type type) where T : class
		{
			return _container.GetInstance(type) as T;
		}

		public static IContainer CreateChildContainer()
		{
			return _container.CreateChildContainer();
		}

		public static void Register<T>(IContainer container, object instance) where T : class
		{
			_container.Configure(ce => ce.For<T>().Use((T) instance));
		}
	}
	public static class Constants
	{
		public static class AzureQueueNames
		{
			public const string AdHocJobQueue = "adhocjobs";
		}
	}

	public class AadHocJobParams
	{
		public string JobName { get; set; }
		public string Message { get; set; }
	}


	public class TriggeredJobs
	{
		public void ProcessQueueMessage([QueueTrigger(Constants.AzureQueueNames.AdHocJobQueue)] AadHocJobParams jobParams, TextWriter logger)
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

		public void TickTockJob([TimerTrigger("0 */1 * * * *")]
			AadHocJobParams jobParams, TextWriter logger)
		{
			RunJob(typeof(TickTockJob), logger);
		}

		private void RunJob(Type jobtype, TextWriter logger)
		{
			var thisContainer = ServiceLocator.CreateChildContainer();
			ServiceLocator.Register<TextWriter>(thisContainer, logger);
			logger.WriteLine($"found job type {jobtype.FullName}... resolving from IoC...");
			var job = (IJob)thisContainer.GetInstance(jobtype);
			logger.WriteLine($"... and off we go!!");
			job.Run();
		}

		private IEnumerable<Type> GetAllPossibleJobs()
		{
			return Assembly.GetExecutingAssembly().GetTypes().Where(t => typeof(IJob).IsAssignableFrom(t));
		}
	}

	public class Test1 : IJob
	{
		private readonly TextWriter _logger;
		public Test1(TextWriter logger)
		{
			_logger = logger;
		}

		public Task Run()
		{
			JobLogger.WriteLine(_logger, "Whhhoooooo!!!!");
			return Task.CompletedTask;
		}
	}

	public class Test2 : IJob
	{
		private readonly TextWriter _logger;
		public Test2(TextWriter logger)
		{
			_logger = logger;
		}

		public Task Run()
		{
			JobLogger.WriteLine(_logger, "Whhhaaaaaaa!!!!");
			return Task.CompletedTask;
		}
	}

	public class BadRobot : IJob
	{
		private readonly TextWriter _logger;
		public BadRobot(TextWriter logger)
		{
			_logger = logger;
		}

		public Task Run()
		{
			throw new Exception("Abandon ship!");
		}
	}

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

	static class JobLogger
	{
		public static void WriteLine(TextWriter textWriter, string message)
		{
			Console.WriteLine(message);
			textWriter.WriteLine(message);
		}
	}

}
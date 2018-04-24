using Microsoft.Azure.WebJobs;
using SFA.DAS.EAS.Account.Worker.Infrastructure.Interfaces;
using SFA.DAS.EAS.Domain.Interfaces;
using Microsoft.Azure.WebJobs.Host;
using StructureMap;


namespace SFA.DAS.EAS.Account.Worker.Infrastructure
{
	public class JobHostFactory : IJobHostFactory
	{
		private readonly IWebJobConfiguration _webJobConfiguration;

		public JobHostFactory(IWebJobConfiguration webJobConfiguration)
		{
			_webJobConfiguration = webJobConfiguration;
		}

		public JobHost CreateJobHost()
		{
			JobHostConfiguration config = new JobHostConfiguration
			{
				DashboardConnectionString = _webJobConfiguration.DashboardConnectionString,
				StorageConnectionString = _webJobConfiguration.StorageConnectionString
            };

            config.Tracing.Tracers.Add(ServiceLocator.Get<DasWebJobTraceWriter>());
			JobHost host = new JobHost(config);

			return host;
		}
	}
}
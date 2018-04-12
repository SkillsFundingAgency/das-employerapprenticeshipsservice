using Microsoft.Azure.WebJobs;
using SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure.Interfaces;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Infrastructure
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

			JobHost host = new JobHost(config);

			return host;
		}
	}
}
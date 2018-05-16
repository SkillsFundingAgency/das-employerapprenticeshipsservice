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
	    private readonly DasWebJobTraceWriter _traceWriter;

        public JobHostFactory(IWebJobConfiguration webJobConfiguration, DasWebJobTraceWriter traceWriter)
		{
			_webJobConfiguration = webJobConfiguration;
		    _traceWriter = traceWriter;
		}

		public JobHost CreateJobHost()
		{
			JobHostConfiguration config = new JobHostConfiguration
			{
				DashboardConnectionString = _webJobConfiguration.DashboardConnectionString,
				StorageConnectionString = _webJobConfiguration.StorageConnectionString
            };

            config.Tracing.Tracers.Add(_traceWriter);
			JobHost host = new JobHost(config);

			return host;
		}
	}
}
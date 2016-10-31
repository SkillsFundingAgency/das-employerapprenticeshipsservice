using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.Logging;
using SFA.DAS.EAS.LevyAggregationProvider.Worker.DependencyResolution;
using SFA.DAS.EAS.LevyAggregationProvider.Worker.Providers;
using StructureMap;

namespace SFA.DAS.EAS.LevyAggregationProvider.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private Container _container;

        public override void Run()
        {
            LoggingConfig.ConfigureLogging();

            _container = new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
                c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider"));
                c.Policies.Add<LoggingPolicy>();
                c.Policies.Add(new MessagePolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
                c.AddRegistry<DefaultRegistry>();
            });

            Trace.TraceInformation("SFA.DAS.LevyAggregationProvider.Worker is running");

            try
            {
                this.RunAsync(this.cancellationTokenSource.Token).Wait();
            }
            finally
            {
                this.runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;
            
            var result = base.OnStart();

            Trace.TraceInformation("SFA.DAS.LevyAggregationProvider.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SFA.DAS.LevyAggregationProvider.Worker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SFA.DAS.LevyAggregationProvider.Worker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var manager = _container.GetInstance<LevyAggregationManager>();

            while (!cancellationToken.IsCancellationRequested)
            {
                await manager.Process();

                Trace.TraceInformation("Working");
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}

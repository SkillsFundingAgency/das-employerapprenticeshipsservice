using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Configuration;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.DependencyResolution;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Logging;
using StructureMap;
using WorkerRole1.DependencyResolution;
using WorkerRole1.Providers;

namespace WorkerRole1
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;

        public override void Run()
        {
            LoggingConfig.ConfigureLogging();

            Trace.TraceInformation("SFA.DAS.LevyMonthlyUpdate.Worker is running");

            try
            {
                RunAsync(cancellationTokenSource.Token).Wait();
            }
            finally
            {
                runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            ServicePointManager.DefaultConnectionLimit = 12;

            _container = new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider"));
                c.Policies.Add(new ConfigurationPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
                c.Policies.Add<LoggingPolicy>();
                c.Policies.Add(new MessagePolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
                c.AddRegistry<DefaultRegistry>();
            });



            var result = base.OnStart();

            Trace.TraceInformation("SFA.DAS.LevyMonthlyUpdate.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SFA.DAS.LevyMonthlyUpdate.Worker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SFA.DAS.LevyMonthlyUpdate.Worker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var monthlyLevyUpdate = _container.GetInstance<IMonthlyLevyUpdate>();

            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                //await monthlyLevyUpdate.Handle();
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}

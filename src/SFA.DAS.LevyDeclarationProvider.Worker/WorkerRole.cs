using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.Logging;
using SFA.DAS.EAS.LevyDeclarationProvider.Worker.DependencyResolution;
using SFA.DAS.EAS.LevyDeclarationProvider.Worker.Providers;
using StructureMap;

namespace SFA.DAS.EAS.LevyDeclarationProvider.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;

        public override void Run()
        {
            LoggingConfig.ConfigureLogging();

            Trace.TraceInformation("SFA.DAS.LevyDeclarationProvider.Worker is running");

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
                c.Policies.Add(new ConfigurationPolicy<TokenServiceApiClientConfiguration>("SFA.DAS.TokenServiceApiClient"));
                c.Policies.Add<LoggingPolicy>();
                c.Policies.Add(new MessagePolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
                c.Policies.Add(new ExecutionPolicyPolicy());
                c.AddRegistry<DefaultRegistry>();
            });



            var result = base.OnStart();

            Trace.TraceInformation("SFA.DAS.LevyDeclarationProvider.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SFA.DAS.LevyDeclarationProvider.Worker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SFA.DAS.LevyDeclarationProvider.Worker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var levyDeclaration = _container.GetInstance<ILevyDeclaration>();

            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");
                
                await levyDeclaration.Handle();
                await Task.Delay(1000, cancellationToken);
            }
        }
    }
}

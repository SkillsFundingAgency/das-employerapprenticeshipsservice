using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.LevyAggregationProvider.Worker.DependencyResolution;
using SFA.DAS.LevyAggregationProvider.Worker.Providers;
using StructureMap;

namespace SFA.DAS.LevyAggregationProvider.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private Container _container;

        public override void Run()
        {
            var registry = new DefaultRegistry();

            _container = new Container(registry);

            _container.AssertConfigurationIsValid();

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
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at http://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

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

            // TODO: Replace the following with your own logic.
            while (!cancellationToken.IsCancellationRequested)
            {
                await manager.Process();

                Trace.TraceInformation("Working");
                await Task.Delay(1000);
            }
        }
    }
}

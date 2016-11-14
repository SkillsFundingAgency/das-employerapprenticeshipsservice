using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure;
using Microsoft.WindowsAzure.Diagnostics;
using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.PaymentProvider.Worker.DependencyResolution;
using SFA.DAS.EAS.PaymentProvider.Worker.Providers;
using StructureMap;

namespace SFA.DAS.EAS.PaymentProvider.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;

        public override void Run()
        {
            Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker is running");

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
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            bool result = base.OnStart();

            Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker has been started");

            _container = new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<PaymentProviderConfiguration>("SFA.DAS.PaymentProvider"));
                c.Policies.Add<LoggingPolicy>();
                c.Policies.Add(new MessagePolicy<PaymentProviderConfiguration>("SFA.DAS.PaymentProvider"));
                c.AddRegistry<DefaultRegistry>();
            });

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker is stopping");

            this.cancellationTokenSource.Cancel();
            this.runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker has stopped");
        }

        private async Task RunAsync(CancellationToken cancellationToken)
        {
            var paymentDataProcessor = _container.GetInstance<IPaymentDataProcessor>();

            while (!cancellationToken.IsCancellationRequested)
            {
                Trace.TraceInformation("Working");

                await paymentDataProcessor.Handle();
                await Task.Delay(1000);
            }
        }
    }
}

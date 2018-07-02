using Microsoft.WindowsAzure.ServiceRuntime;
using NLog;
using SFA.DAS.EAS.Application.DependencyResolution;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.Extensions;
using SFA.DAS.EAS.Infrastructure.Logging;
using SFA.DAS.EAS.PaymentProvider.Worker.DependencyResolution;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using StructureMap;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        protected IContainer Container;

        public override void Run()
        {
            LoggingConfig.ConfigureLogging();

            Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker is running");

            try
            {
                var processors = Container.GetAllInstances<IMessageProcessor>().ToArray();

                var processorTasks = new Task[processors.Length];

                for (var index = 0; index < processors.Length; index++)
                {
                    processorTasks[index] = processors[index].RunAsync(_cancellationTokenSource);
                }

                Task.WaitAll(processorTasks, _cancellationTokenSource.Token);
            }
			catch (OperationCanceledException)
            {
                Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker has been cancelled");
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, ex.GetMessage());
                throw;
            }
            finally
            {
                _runCompleteEvent.Set();
            }
        }

        public override bool OnStart()
        {
            // Set the maximum number of concurrent connections
            ServicePointManager.DefaultConnectionLimit = 12;

            // For information on handling configuration changes
            // see the MSDN topic at https://go.microsoft.com/fwlink/?LinkId=166357.

            var result = base.OnStart();

            Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker has been started");

            Container = new Container(c =>
            {
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<CommitmentsRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<ExecutionPoliciesRegistry>();
                c.AddRegistry<LevyRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<MessageSubscriberRegistry>();
                c.AddRegistry<PaymentsRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker is stopping");

            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker has stopped");
        }
    }
}

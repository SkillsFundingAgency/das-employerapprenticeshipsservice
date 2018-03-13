using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.Logging;
using SFA.DAS.EAS.PaymentProvider.Worker.DependencyResolution;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using StructureMap;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.PaymentProvider.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;

        public override void Run()
        {
            LoggingConfig.ConfigureLogging();
            Trace.TraceInformation("SFA.DAS.EAS.PaymentProvider.Worker is running");

            try
            {
                var processors = _container.GetAllInstances<IMessageProcessor>().ToArray();

                var processorTasks = new Task[processors.Length];

                for (var index = 0; index < processors.Length; index++)
                {
                    processorTasks[index] = processors[index].RunAsync(_cancellationTokenSource.Token);
                }

                Task.WaitAll(processorTasks);
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

            _container = new Container(c =>
            {
                c.Policies.Add(new ConfigurationPolicy<LevyDeclarationProviderConfiguration>("SFA.DAS.LevyAggregationProvider"));
                c.Policies.Add(new ConfigurationPolicy<PaymentProviderConfiguration>("SFA.DAS.PaymentProvider"));
                c.Policies.Add(new ConfigurationPolicy<PaymentsApiClientConfiguration>("SFA.DAS.PaymentsAPI"));
                c.Policies.Add(new ConfigurationPolicy<CommitmentsApiClientConfiguration>("SFA.DAS.CommitmentsAPI"));
                c.Policies.Add(new TopicMessagePublisherPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService", "1.0", new NLogLogger(typeof(TopicMessagePublisher))));
                c.Policies.Add(new MessageSubscriberPolicy<EmployerApprenticeshipsServiceConfiguration>("SFA.DAS.EmployerApprenticeshipsService"));
                c.Policies.Add(new ExecutionPolicyPolicy());
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

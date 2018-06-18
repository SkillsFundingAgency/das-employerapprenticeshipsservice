using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.ServiceRuntime;
using NLog;
using SFA.DAS.EAS.Application.DependencyResolution;
using SFA.DAS.EAS.MessageHandlers.Worker.DependencyResolution;
using SFA.DAS.Messaging.Interfaces;
using StructureMap;

namespace SFA.DAS.EAS.MessageHandlers.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);

        public override void Run()
        {
            Logger.Info("SFA.DAS.EAS.MessageHandlers is running");

            try
            {
                var container = new Container(c =>
                {
                    c.AddRegistry<CachesRegistry>();
                    c.AddRegistry<ConfigurationRegistry>();
                    c.AddRegistry<HashingRegistry>();
                    c.AddRegistry<LoggerRegistry>();
                    c.AddRegistry<MapperRegistry>();
                    c.AddRegistry<MediatorRegistry>();
                    c.AddRegistry<MessagePublisherRegistry>();
                    c.AddRegistry<MessageSubscriberRegistry>();
                    c.AddRegistry<RepositoriesRegistry>();
                    c.AddRegistry<NotificationsRegistry>();
                    c.AddRegistry<DefaultRegistry>();
                });

                var messageProcessors = container.GetAllInstances<IMessageProcessor>().ToList();
                var tasks = messageProcessors.Select(p => p.RunAsync(_cancellationTokenSource)).ToArray();

                Task.WaitAll(tasks);
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex);
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

            Logger.Info("SFA.DAS.EAS.MessageHandlers has been started");

            return result;
        }

        public override void OnStop()
        {
            Logger.Info("SFA.DAS.EAS.MessageHandlers is stopping");

            _cancellationTokenSource.Cancel();
            _runCompleteEvent.WaitOne();

            base.OnStop();

            Logger.Info("SFA.DAS.EAS.MessageHandlers has stopped");
        }
    }
}

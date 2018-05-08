using Microsoft.WindowsAzure.ServiceRuntime;
using SFA.DAS.EAS.Application.DependencyResolution;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Infrastructure.DependencyResolution;
using SFA.DAS.EAS.Infrastructure.Extensions;
using SFA.DAS.EAS.Infrastructure.Logging;
using SFA.DAS.EAS.LevyDeclarationProvider.Worker.DependencyResolution;
using SFA.DAS.EAS.LevyDeclarationProvider.Worker.Providers;
using SFA.DAS.Messaging.AzureServiceBus;
using SFA.DAS.Messaging.AzureServiceBus.StructureMap;
using SFA.DAS.NLog.Logger;
using StructureMap;
using System;
using System.Diagnostics;
using System.Net;
using System.Threading;

namespace SFA.DAS.EAS.LevyDeclarationProvider.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ManualResetEvent _runCompleteEvent = new ManualResetEvent(false);
        private IContainer _container;

        public override void Run()
        {
            LoggingConfig.ConfigureLogging();

            Trace.TraceInformation("SFA.DAS.LevyDeclarationProvider.Worker is running");

            try
            {
                var levyDeclaration = _container.GetInstance<ILevyDeclaration>();
                levyDeclaration.RunAsync(_cancellationTokenSource.Token).Wait();
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
            ServicePointManager.DefaultConnectionLimit = 12;

            _container = new Container(c =>
            {
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<EventsRegistry>();
                c.AddRegistry<ExecutionPoliciesRegistry>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<LevyRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<MessagePublisherRegistry>();
                c.AddRegistry<MessageSubscriberRegistry>();
                c.AddRegistry<RepositoriesRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<DefaultRegistry>();
            });

            var result = base.OnStart();

            Trace.TraceInformation("SFA.DAS.LevyDeclarationProvider.Worker has been started");

            return result;
        }

        public override void OnStop()
        {
            Trace.TraceInformation("SFA.DAS.LevyDeclarationProvider.Worker is stopping");

            this._cancellationTokenSource.Cancel();
            this._runCompleteEvent.WaitOne();

            base.OnStop();

            Trace.TraceInformation("SFA.DAS.LevyDeclarationProvider.Worker has stopped");
        }
    }
}

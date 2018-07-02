using NLog;
using SFA.DAS.EAS.Infrastructure.Extensions;
using SFA.DAS.EAS.Infrastructure.Logging;
using SFA.DAS.EAS.LevyAccountUpdater.WebJob.DependencyResolution;
using SFA.DAS.EAS.LevyAccountUpdater.WebJob.Updater;
using System;

namespace SFA.DAS.EAS.LevyAccountUpdater.WebJob
{
    class Program
    {
        private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            LoggingConfig.ConfigureLogging();

            var container = IoC.Initialize();

            try
            {
                var updater = container.GetInstance<IAccountUpdater>();

                updater.RunUpdate().Wait();
            }
            catch (Exception ex)
            {
                Logger.Fatal(ex, ex.GetMessage());
                throw;
            }

        }
    }

}

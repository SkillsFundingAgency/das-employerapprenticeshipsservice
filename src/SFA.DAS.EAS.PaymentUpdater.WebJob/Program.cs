using SFA.DAS.EAS.Infrastructure.Logging;
using SFA.DAS.EAS.PaymentUpdater.WebJob.DependencyResolution;
using SFA.DAS.EAS.PaymentUpdater.WebJob.Updater;

namespace SFA.DAS.EAS.PaymentUpdater.WebJob
{
    // To learn more about Microsoft Azure WebJobs SDK, please see http://go.microsoft.com/fwlink/?LinkID=320976
    public class Program
    {
        // Please set the following connection strings in app.config for this WebJob to run:
        // AzureWebJobsDashboard and AzureWebJobsStorage
        static void Main()
        {
            LoggingConfig.ConfigureLogging();

            var container = IoC.Initialize();

            var paymentUpdater = container.GetInstance<IPaymentProcessor>();

            paymentUpdater.RunUpdate().Wait();
        }
    }
}

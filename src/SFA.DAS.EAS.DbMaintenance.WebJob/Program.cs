using System;
using System.Diagnostics;
using SFA.DAS.EAS.DbMaintenance.WebJob.DependencyResolution;
using SFA.DAS.EAS.DbMaintenance.WebJob.Jobs;
using SFA.DAS.EAS.DbMaintenance.WebJob.Jobs.GenerateAgreements;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.DbMaintenance.WebJob
{
    public class Program
    {
        public static void Main()
        {
            var container = IoC.Initialize();
            var logger = container.GetInstance<ILog>();

            try
            {
                var job = container.GetInstance<GenerateAgreementsJob>() as IJob;
                var jobTypeName = job.GetType().Name;
                logger.Info($"Job '{jobTypeName}' started.");
                job.Run().Wait();
                logger.Info($"Job '{jobTypeName}' finished.");
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Jobs aborted.");
            }

            if (Debugger.IsAttached)
            {
                Console.WriteLine("Press enter to close...");
                Console.ReadLine();
            }
        }
    }
}
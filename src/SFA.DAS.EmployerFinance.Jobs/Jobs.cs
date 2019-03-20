using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Host;
using NServiceBus;
using SFA.DAS.EmployerFinance.Jobs.DependencyResolution;
using SFA.DAS.EmployerFinance.Messages.Commands;
using SFA.DAS.NLog.Logger;
using SFA.DAS.NServiceBus.ClientOutbox;

namespace SFA.DAS.EmployerFinance.Jobs
{
    public static class Jobs
    {
        public static Task ImportLevyDeclarations([TimerTrigger("0 0 15 21 * *")] TimerInfo timer, TraceWriter logger)
        {
            ILog nLogger = null;

            try
            {
                nLogger = ServiceLocator.GetInstance<ILog>();

                nLogger.Info("Getting message session for import levy declarations job");
                var messageSession = ServiceLocator.GetInstance<IMessageSession>();
                
                nLogger.Info("Creating new import levy declarations command for job");
                return messageSession.Send(new ImportLevyDeclarationsCommand());
            }
            catch (Exception e)
            {
                nLogger?.Error(e, "Failed to create new import levy declarations command for job");
                throw;
            }
        }

        public static Task ImportPayments([TimerTrigger("0 0 * * * *")] TimerInfo timer, TraceWriter logger)
        {
            ILog nLogger = null;

            try
            {
                nLogger = ServiceLocator.GetInstance<ILog>();
                
                nLogger.Info("Getting message session for import payments job");
                var messageSession = ServiceLocator.GetInstance<IMessageSession>();
              
                nLogger.Info("Creating new import payment command for job");
                return messageSession.Send(new ImportPaymentsCommand());
            }
            catch (Exception e)
            {
                nLogger?.Error(e, "Failed to create new import payment command for job");
                throw;
            }
        }

        public static Task ProcessOutboxMessages([TimerTrigger("0 */10 * * * *")] TimerInfo timer, TraceWriter logger)
        {
            ILog nLogger = null;

            try
            {
                nLogger = ServiceLocator.GetInstance<ILog>();
                
                nLogger.Info("Getting client outbox message job");
                var job = ServiceLocator.GetInstance<IProcessClientOutboxMessagesJob>();
            
                nLogger.Info("Running client outbox message job");   
                return job.RunAsync();
            }
            catch (Exception e)
            {
                nLogger?.Error(e, "Failed run client outbox message job");
                throw;
            }
        }
    }
}
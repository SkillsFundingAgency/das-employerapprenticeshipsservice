using System.Threading.Tasks;
using SFA.DAS.EAS.DbMaintenance.WebJob.IdProcessor;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.DbMaintenance.WebJob.Jobs.GenerateAgreements
{
    public class GenerateAgreementsJob : IJob
    {
        private readonly IBroker _idBroker;
        private readonly IProcessor _accountProcessor;
        private readonly IIdProvider _accountIdProvider;
        private readonly ILog _log;

        public GenerateAgreementsJob(
            IBroker idBroker, 
            IProcessor accountProcessor, 
            IIdProvider accountIdProvider,
            ILog log)
        {
            _idBroker = idBroker;
            _accountProcessor = accountProcessor;
            _accountIdProvider = accountIdProvider;
            _log = log;
        }

        public async Task Run()
        {
            _log.Info($"Starting processing of accounts {_accountIdProvider.GetType().Name}");

            var processInfo = await _idBroker.ProcessAsync(_accountIdProvider, _accountProcessor);

            _log.Info($"Finished processing for {_accountIdProvider.GetType().Name}. " +
                      $" processed:{processInfo.IdsProcessed} " +
                      $" last-processed-id:{processInfo.LastProcessedId} " +
                      $" batch-size:{processInfo.BatchSize} " +
                      $" status:{processInfo.State} " +
                      $" start-time(UTC):{processInfo.StartTime} " +
                      $" end-time(UTC):{processInfo.EndTime}" +
                      $" duration(seconds):{(processInfo.EndTime - processInfo.StartTime).TotalSeconds}");
        }
    }
}

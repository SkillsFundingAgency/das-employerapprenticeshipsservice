using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Worker.IdProcessor;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.Jobs.GenerateAgreements
{
    public class GenerateAgreementsJob : IJob
    {
        private readonly IIdBroker _idIdBroker;
        private readonly IProcessor _accountProcessor;
        private readonly IIdProvider _accountIdProvider;
        private readonly ILog _log;

        public GenerateAgreementsJob(
            IIdBroker idIdBroker,
            GenerateAgreementsIdProcessor accountProcessor,
            GenerateAgreementsIdProvider accountIdProvider,
            ILog log)
        {
            _idIdBroker = idIdBroker;
            _accountProcessor = accountProcessor;
            _accountIdProvider = accountIdProvider;
            _log = log;
        }

        public async Task Run()
        {
            _log.Info($"Starting processing of accounts {_accountIdProvider.GetType().Name}");

            var processInfo = await _idIdBroker.ProcessAsync(_accountIdProvider, _accountProcessor);

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

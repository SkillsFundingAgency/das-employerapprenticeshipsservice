using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SFA.DAS.EAS.Account.Worker.Data;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Account.Worker.Jobs
{
    public class UpdateAccountPublicHashedIdsJob : IJob
    {
        private readonly IAccountMaintenanceRepository _accountMaintenanceRepository;
        private readonly ILog _logger;
        private readonly IPublicHashingService _publicHashingService;

        public UpdateAccountPublicHashedIdsJob(IAccountMaintenanceRepository accountMaintenanceRepository, ILog logger, IPublicHashingService publicHashingService)
        {
            _accountMaintenanceRepository = accountMaintenanceRepository;
            _logger = logger;
            _publicHashingService = publicHashingService;
        }

        public async Task Run()
        {
            var ids = await _accountMaintenanceRepository.GetAccountsMissingPublicHashedId();
            var publicHashedIds = ids.Select(i => new KeyValuePair<long, string>(i, _publicHashingService.HashValue(i)));

            _logger.Info($"Updating '{ids.Count}' accounts.");

            await _accountMaintenanceRepository.UpdateAccountPublicHashedIds(publicHashedIds);
        }
    }
}

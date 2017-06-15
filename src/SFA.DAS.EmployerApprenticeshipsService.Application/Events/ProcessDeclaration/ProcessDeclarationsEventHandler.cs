using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Events.ProcessDeclaration
{
    public class ProcessDeclarationsEventHandler : IAsyncNotificationHandler<ProcessDeclarationsEvent>
    {
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly ILog _logger;

        public ProcessDeclarationsEventHandler(IDasLevyRepository dasLevyRepository, ILog logger)
        {
            _dasLevyRepository = dasLevyRepository;
            _logger = logger;
        }

        public async Task Handle(ProcessDeclarationsEvent notification)
        {
            await _dasLevyRepository.ProcessDeclarations();

            _logger.Info("Process Declarations Called");
        }
    }
}
using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Events
{
    public class ProcessDeclarationsEventHandler : IAsyncNotificationHandler<ProcessDeclarationsEvent>
    {
        private readonly IDasLevyRepository _dasLevyRepository;
        private readonly ILogger _logger;

        public ProcessDeclarationsEventHandler(IDasLevyRepository dasLevyRepository, ILogger logger)
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
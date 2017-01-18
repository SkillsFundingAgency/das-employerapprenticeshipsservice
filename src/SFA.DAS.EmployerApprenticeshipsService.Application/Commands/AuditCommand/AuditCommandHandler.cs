using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.AuditCommand
{
    public class AuditCommandHandler : AsyncRequestHandler<AuditCommand>
    {
        private readonly IAuditService _auditService;
        private readonly IValidator<AuditCommand> _validator;

        public AuditCommandHandler(IAuditService auditService, IValidator<AuditCommand> validator)
        {
            _auditService = auditService;
            _validator = validator;
        }

        protected override async Task HandleCore(AuditCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            await _auditService.SendAuditMessage(message.EasAuditMessage);
        }
    }
}
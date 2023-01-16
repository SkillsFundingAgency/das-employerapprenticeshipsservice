using System.Threading;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Commands.AuditCommand;

public class CreateAuditCommandHandler : IRequestHandler<CreateAuditCommand>
{
    private readonly IAuditService _auditService;
    private readonly IValidator<CreateAuditCommand> _validator;

    public CreateAuditCommandHandler(IAuditService auditService, IValidator<CreateAuditCommand> validator)
    {
        _auditService = auditService;
        _validator = validator;
    }

    public async Task<Unit> Handle(CreateAuditCommand message, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(message);

        if (!validationResult.IsValid())
        {
            throw new InvalidRequestException(validationResult.ValidationDictionary);
        }

        await _auditService.SendAuditMessage(message.EasAuditMessage);

        return default;
    }
}
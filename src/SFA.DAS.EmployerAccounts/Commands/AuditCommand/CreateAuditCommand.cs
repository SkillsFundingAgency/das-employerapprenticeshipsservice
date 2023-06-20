using SFA.DAS.EmployerAccounts.Audit.Types;

namespace SFA.DAS.EmployerAccounts.Commands.AuditCommand;

public class CreateAuditCommand : IRequest
{
    public AuditMessage EasAuditMessage { get; set; }
}
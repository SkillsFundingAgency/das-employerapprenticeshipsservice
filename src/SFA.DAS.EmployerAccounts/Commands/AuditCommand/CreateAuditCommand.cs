using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.AuditCommand;

public class CreateAuditCommand : IRequest
{
    public EasAuditMessage EasAuditMessage { get; set; }
}
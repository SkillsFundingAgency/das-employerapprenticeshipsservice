using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Commands.AuditCommand;

public class CreateAuditCommand : IAsyncRequest
{
    public EasAuditMessage EasAuditMessage { get; set; }
}
using SFA.DAS.EmployerAccounts.Audit.Types;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IAuditService
{
    Task SendAuditMessage(AuditMessage message);
}
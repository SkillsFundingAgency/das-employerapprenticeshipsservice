using SFA.DAS.EmployerAccounts.Audit.Types;

namespace SFA.DAS.EmployerAccounts.Audit;
public interface IAuditApiClient
{
    Task Audit(AuditMessage message);
}

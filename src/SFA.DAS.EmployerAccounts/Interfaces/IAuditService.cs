using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Interfaces;

public interface IAuditService
{
    Task SendAuditMessage(EasAuditMessage message);
}
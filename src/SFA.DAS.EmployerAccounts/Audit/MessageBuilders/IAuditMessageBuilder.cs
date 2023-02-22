using SFA.DAS.EmployerAccounts.Audit.Types;

namespace SFA.DAS.EmployerAccounts.Audit.MessageBuilders;
public interface IAuditMessageBuilder
{
    void Build(AuditMessage message);
}

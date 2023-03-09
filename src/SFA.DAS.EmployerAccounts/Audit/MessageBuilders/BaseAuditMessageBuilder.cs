using System.Reflection;
using SFA.DAS.EmployerAccounts.Audit.Types;

namespace SFA.DAS.EmployerAccounts.Audit.MessageBuilders
{
    public class BaseAuditMessageBuilder : IAuditMessageBuilder
    {
        public void Build(AuditMessage message)
        {
            var name = Assembly.GetExecutingAssembly();

            message.Source = new Source
            {
                System = "EmployerAccounts",
                Component = "EmployerAccounts-Web",
                Version = name.GetName().Version.ToString()
            };

            message.ChangeAt = DateTime.UtcNow;
        }
    }
}

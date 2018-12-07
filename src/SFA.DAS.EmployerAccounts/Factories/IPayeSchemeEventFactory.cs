using SFA.DAS.EmployerAccounts.Models.PayeScheme;

namespace SFA.DAS.EmployerAccounts.Factories
{
    public interface IPayeSchemeEventFactory
    {
        PayeSchemeAddedEvent CreatePayeSchemeAddedEvent(string hashedAccountId, string payeScheme);
        PayeSchemeRemovedEvent CreatePayeSchemeRemovedEvent(string hashedAccountId, string payeScheme);
    }
}

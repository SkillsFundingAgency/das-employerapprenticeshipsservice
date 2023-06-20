using SFA.DAS.EmployerAccounts.Events.PayeScheme;
namespace SFA.DAS.EmployerAccounts.Factories;

public interface IPayeSchemeEventFactory
{
    PayeSchemeAddedEvent CreatePayeSchemeAddedEvent(string hashedAccountId, string payeSchemeRef);
    PayeSchemeRemovedEvent CreatePayeSchemeRemovedEvent(string hashedAccountId, string payeSchemeRef);
}
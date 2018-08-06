using SFA.DAS.EAS.Account.Api.Types.Events.Levy;

namespace SFA.DAS.EmployerFinance.Factories
{
    public interface ILevyEventFactory
    {
        LevyDeclarationUpdatedEvent CreateDeclarationUpdatedEvent(string hashedAccountId, string payrollYear, short? payrollMonth);
    }
}

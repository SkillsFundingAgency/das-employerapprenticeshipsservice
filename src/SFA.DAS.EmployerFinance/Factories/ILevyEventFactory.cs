using SFA.DAS.EmployerFinance.Events;

namespace SFA.DAS.EmployerFinance.Factories
{
    public interface ILevyEventFactory
    {
        LevyDeclarationUpdatedEvent CreateDeclarationUpdatedEvent(string hashedAccountId, string payrollYear, short? payrollMonth);
    }
}

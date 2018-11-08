using SFA.DAS.EmployerFinance.Events;

namespace SFA.DAS.EmployerFinance.Factories
{
    public class LevyEventFactory : ILevyEventFactory
    {
        public LevyDeclarationUpdatedEvent CreateDeclarationUpdatedEvent(string hashedAccountId, string payrollYear, short? payrollMonth)
        {
            return new LevyDeclarationUpdatedEvent
            {
                ResourceUri = $"api/accounts/{hashedAccountId}/levy/{payrollYear}/{payrollMonth}"
            };
        }
    }
}

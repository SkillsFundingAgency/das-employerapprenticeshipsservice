using SFA.DAS.EAS.Account.Api.Types.Events.Levy;

namespace SFA.DAS.EAS.Application.Factories
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

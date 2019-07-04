using SFA.DAS.EAS.Portal.Client.Types;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Portal.Client.TestHarness.Scenarios
{
    public class GetAccountScenario
    {
        private readonly IPortalClient _portalClient;

        public GetAccountScenario(IPortalClient portalClient)
        {
            _portalClient = portalClient;
        }

        public async Task<Account> Run()
        {
            const long accountId = 1L;

            return await _portalClient.GetAccount(accountId,false);
        }
    }
}

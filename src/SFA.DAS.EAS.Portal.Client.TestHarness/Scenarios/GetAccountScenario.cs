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

        public async Task Run()
        {
            const long accountId = 1337L;

            var accountDto = await _portalClient.GetAccount(accountId);
        }
    }
}
using System.Threading.Tasks;
using SFA.DAS.EAS.Portal.Client.Models.Concrete;

namespace SFA.DAS.EAS.Portal.Client.TestHarness.Scenarios
{
    public class GetAccountScenario
    {
        private readonly IPortalClient _portalClient;

        public GetAccountScenario(IPortalClient portalClient)
        {
            _portalClient = portalClient;
        }

        public async Task<AccountDto> Run()
        {
            const long accountId = 1337L;

            return await _portalClient.GetAccount(accountId);
        }
    }
}
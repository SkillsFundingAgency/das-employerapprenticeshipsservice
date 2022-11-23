using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Services.EmployerFinanceApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accounts/{hashedAccountId}/transfers/connections")]
    public class TransferConnectionsController : ApiController
    {
        private readonly IEmployerFinanceApiService _apiService;

        public TransferConnectionsController(IEmployerFinanceApiService apiService)
        {
            _apiService = apiService;
        }

        [Route]
        public async Task<IHttpActionResult> GetTransferConnections(string hashedAccountId)
        {
            return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/transfers/connections"));
        }
    }
}
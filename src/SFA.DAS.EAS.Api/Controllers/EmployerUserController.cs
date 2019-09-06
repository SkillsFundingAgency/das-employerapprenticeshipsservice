using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/user/{userRef}")]
    public class EmployerUserController : ApiController
    {
        private readonly IEmployerAccountsApiService _apiService;

        public EmployerUserController(IEmployerAccountsApiService apiService)
        {
            _apiService = apiService;
        }

        [Route("accounts", Name = "Accounts")]
        [ApiAuthorize(Roles = "ReadUserAccounts")]
        [HttpGet]
        public async Task<IHttpActionResult> GetUserAccounts(string userRef)
        {
            return Ok(await _apiService.Redirect($"/api/user/{userRef}/accounts"));
        }
    }
}
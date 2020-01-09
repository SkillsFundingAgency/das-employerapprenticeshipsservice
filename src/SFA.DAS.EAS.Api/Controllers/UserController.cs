using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/user")]
    public class UserController : ApiController
    {
        private readonly IEmployerAccountsApiService _apiService;

        public UserController(IEmployerAccountsApiService apiService)
        {
            _apiService = apiService;
        }

        [Route("")]
        public async Task<IHttpActionResult> Get(string email)
        {
            try
            {
                return Ok(await _apiService.Redirect($"/api/user?email={email}"));
            }
            catch
            {
                return NotFound();
            }
        }
    }
}

using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [RoutePrefix("api/accounts/{hashedAccountId}/payeschemes")]
    public class AccountPayeSchemesController : ApiController
    {
        private readonly IEmployerAccountsApiService _apiService;

        public AccountPayeSchemesController(IEmployerAccountsApiService apiService)
        {
            _apiService = apiService;
        }

        [Route("", Name = "GetPayeSchemes")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeSchemes(string hashedAccountId)
        {
            return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/payeschemes"));
        }

        [Route("{payeschemeref}", Name = "GetPayeScheme")]
        [ApiAuthorize(Roles = "ReadAllEmployerAccountBalances")]
        [HttpGet]
        public async Task<IHttpActionResult> GetPayeScheme(string hashedAccountId, string payeSchemeRef)
        {
            return Ok(await _apiService.Redirect($"/api/accounts/{hashedAccountId}/payeschemes/{HttpUtility.UrlEncode(payeSchemeRef)}"));
        }
    }
}
using System.Threading.Tasks;
using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Services.EmployerAccountsApi;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accountlegalentities")]
    public class AccountLegalEntitiesController : ApiController
    {      
        private readonly IEmployerAccountsApiService _apiService;

        public AccountLegalEntitiesController(IEmployerAccountsApiService apiService)
        {
            _apiService = apiService;
        }

        [Route]
        public async Task<IHttpActionResult> Get(int? pageSize, int? pageNumber)
        {
            return Ok(await _apiService.Redirect($"/api/accountlegalentities?{(pageSize.HasValue ? "pageSize=" + pageSize + "&" : "")}{(pageNumber.HasValue ? "pageNumber=" + pageNumber : "")}"));
        }
    }
}

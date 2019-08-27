using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accountlegalentities")]
    public class AccountLegalEntitiesController : ApiController
    {      
        private readonly EmployerAccountsApiConfiguration _configuration;

        public AccountLegalEntitiesController(EmployerAccountsApiConfiguration configuration)
        {
            _configuration = configuration;
        }

        [Route]
        public IHttpActionResult Get(int? pageSize, int? pageNumber)
        {
            return Redirect(_configuration.BaseUrl + $"/api/accountlegalentities?{(pageSize.HasValue ? "pageSize=" + pageSize + "&" : "")}{(pageNumber.HasValue ? "pageNumber=" + pageNumber : "")}");
        }
    }
}

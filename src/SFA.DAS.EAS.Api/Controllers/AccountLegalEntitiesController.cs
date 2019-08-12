using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accountlegalentities")]
    public class AccountLegalEntitiesController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;


        public AccountLegalEntitiesController(IMediator mediator, EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _mediator = mediator;
            _configuration = configuration;
        }

        [Route]
        public IHttpActionResult Get(int? pageSize, int? pageNumber)
        {
            return Redirect(_configuration.EmployerAccountsApiBaseUrl + $"/api/accountlegalentities?{(pageSize.HasValue ? "pageSize=" + pageSize + "&" : "")}{(pageNumber.HasValue ? "pageNumber=" + pageNumber : "")}");
        }
    }
}

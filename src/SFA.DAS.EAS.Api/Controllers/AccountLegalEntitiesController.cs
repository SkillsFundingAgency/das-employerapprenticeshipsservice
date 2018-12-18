using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities.Api;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accountlegalentities")]
    public class AccountLegalEntitiesController : RedirectController
    {
        public AccountLegalEntitiesController(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        [Route]
        public IHttpActionResult Get([FromUri] GetAccountLegalEntitiesQuery query)
        {
            return RedirectToEmployerAccountsApi();
        }
    }
}

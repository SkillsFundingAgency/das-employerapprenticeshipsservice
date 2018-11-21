using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Extensions;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities.Api;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    //[ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accountlegalentities")]
    public class AccountLegalEntitiesController : ApiController
    {
        [Route]
        public IHttpActionResult Get([FromUri] GetAccountLegalEntitiesQuery query)
        {
            return Redirect(Url.EmployerAccountsApiAction(Request.RequestUri.PathAndQuery));
        }
    }
}

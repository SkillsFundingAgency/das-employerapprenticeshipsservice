using System.Web.Http;
using SFA.DAS.EAS.Account.Api.Attributes;
using SFA.DAS.EAS.Domain.Configuration;

namespace SFA.DAS.EAS.Account.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/statistics")]
    public class StatisticsController : RedirectController
    {
        public StatisticsController(EmployerApprenticeshipsServiceConfiguration configuration) : base(configuration)
        {
        }

        [Route("")]
        public IHttpActionResult GetStatistics()
        {
            return RedirectToEmployerAccountsApi();
        }
    }

}

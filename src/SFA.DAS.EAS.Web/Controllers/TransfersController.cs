using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EAS.Web.Extensions;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [ValidateMembership]
    [RoutePrefix("accounts/{HashedAccountId}/transfers")]
    public class TransfersController : Controller
    {
        [Route]
        public ActionResult Index()
        {
            return Redirect(Url.EmployerAccountsAction("transfers"));
        }
    }
}
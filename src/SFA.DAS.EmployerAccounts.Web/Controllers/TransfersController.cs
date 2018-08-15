using SFA.DAS.EmployerAccounts.Web.Extensions;
using System.Web.Mvc;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]

    [RoutePrefix("accounts/{HashedAccountId}/transfers")]
    public class TransfersController : Controller
    {
        [Route]
        public ActionResult Index()
        {
            return Redirect(Url.LegacyEasAccountAction("transfers"));
        }
    }
}
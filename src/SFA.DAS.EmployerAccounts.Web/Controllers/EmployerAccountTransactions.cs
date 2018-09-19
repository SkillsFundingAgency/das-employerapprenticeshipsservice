using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Web.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountTransactionsController : Controller
    {
        [Route("finance")]
        [Route("balance")]
        public ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("finance"));
        }
    }
}
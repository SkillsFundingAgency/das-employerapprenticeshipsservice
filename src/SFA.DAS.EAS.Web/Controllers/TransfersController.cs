using SFA.DAS.EAS.Web.Extensions;
using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;

namespace SFA.DAS.EAS.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}/transfers")]
    public class TransfersController : Controller
    {
        [Route]
        public ActionResult Index()
        {
            return Redirect(Url.EmployerFinanceAction("transfers/connections"));
        }
    }
}
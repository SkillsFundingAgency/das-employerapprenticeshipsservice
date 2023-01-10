using System.Web.Mvc;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Web.Extensions;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountTransactionsController : Microsoft.AspNetCore.Mvc.Controller
    {
        [Route("finance")]
        [Route("balance")]
        public Microsoft.AspNetCore.Mvc.ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.EmployerFinanceAction("finance"));
        }
    }
}
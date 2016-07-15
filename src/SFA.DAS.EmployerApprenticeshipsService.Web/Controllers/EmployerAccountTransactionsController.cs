using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    public class EmployerAccountTransactionsController : Controller
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly EmployerAccountTransactionsOrchestrator _accountTransactionsOrchestrator;

        public EmployerAccountTransactionsController(IOwinWrapper owinWrapper, EmployerAccountTransactionsOrchestrator accountTransactionsOrchestrator)
        {
            _owinWrapper = owinWrapper;
            _accountTransactionsOrchestrator = accountTransactionsOrchestrator;
        }

        // GET: EmployerAccountTransactions
        [Authorize]
        public async Task<ActionResult> Index(int accountId)
        {
            
            var transactionViewResult  = await _accountTransactionsOrchestrator.GetAccountTransactions(accountId);

            if (transactionViewResult.Account == null) { Response.Redirect("/");}

            return View(transactionViewResult.Model);
        }
    }
}
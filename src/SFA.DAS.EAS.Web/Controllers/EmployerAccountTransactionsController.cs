using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountTransactionsController : BaseController
    {
        private readonly EmployerAccountTransactionsOrchestrator _accountTransactionsOrchestrator;

        public EmployerAccountTransactionsController(IOwinWrapper owinWrapper, IFeatureToggle featureToggle, 
            EmployerAccountTransactionsOrchestrator accountTransactionsOrchestrator, IUserWhiteList userWhiteList) 
            : base(owinWrapper, featureToggle)
        {
            _accountTransactionsOrchestrator = accountTransactionsOrchestrator;
        }
        
        [Route("balance")]
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            var transactionViewResult  = await _accountTransactionsOrchestrator.GetAccountTransactions(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            if (transactionViewResult.Data.Account == null)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            transactionViewResult.Data.Model.Data.HashedAccountId = hashedAccountId;
            return View(transactionViewResult.Data.Model);
        }

        [Route("balance/levyDeclaration/details")]
        public async Task<ActionResult> LevyDeclarationDetail(string hashedAccountId, DateTime fromDate, DateTime toDate)
        {
            var viewModel = await _accountTransactionsOrchestrator.FindAccountLevyDeclarationTransactions(hashedAccountId, fromDate, toDate, OwinWrapper.GetClaimValue(@"sub"));

            return View("LevyDeclarationDetail", viewModel);
        }

        [Route("balance/payment/details")]
        public async Task<ActionResult> PaymentDetail(string hashedAccountId, DateTime fromDate, DateTime toDate)
        {
            var viewModel = await _accountTransactionsOrchestrator.FindAccountPaymentTransactions(hashedAccountId, fromDate, toDate, OwinWrapper.GetClaimValue(@"sub"));

            return View("PaymentDetails", viewModel);
        }
    }
}
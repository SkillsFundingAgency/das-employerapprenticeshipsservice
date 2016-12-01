using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{accountHashedId}")]
    public class EmployerAccountTransactionsController : BaseController
    {
        private readonly EmployerAccountTransactionsOrchestrator _accountTransactionsOrchestrator;

        public EmployerAccountTransactionsController(IOwinWrapper owinWrapper, IFeatureToggle featureToggle, 
            EmployerAccountTransactionsOrchestrator accountTransactionsOrchestrator, IUserWhiteList userWhiteList) 
            : base(owinWrapper, featureToggle, userWhiteList)
        {
            _accountTransactionsOrchestrator = accountTransactionsOrchestrator;
        }
        
        [Route("Balance")]
        public async Task<ActionResult> Index(string accountHashedId)
        {
            var transactionViewResult  = await _accountTransactionsOrchestrator.GetAccountTransactions(accountHashedId, OwinWrapper.GetClaimValue(@"sub"));

            if (transactionViewResult.Account == null)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            transactionViewResult.Model.Data.AccountHashedId = accountHashedId;
            return View(transactionViewResult.Model);
        }

        [Route("Balance/LevyDeclarationDetail")]
        public async Task<ActionResult> LevyDeclarationDetail(string accountHashedId, DateTime fromDate, DateTime toDate)
        {
            var viewModel = await _accountTransactionsOrchestrator.FindAccountLevyDeclarationTransactions(accountHashedId, fromDate, toDate, OwinWrapper.GetClaimValue(@"sub"));

            return View("LevyDeclarationDetail", viewModel);
        }

        [Route("Balance/PaymentDetail")]
        public async Task<ActionResult> PaymentDetail(string accountHashedId, DateTime fromDate, DateTime toDate)
        {
            var viewModel = await _accountTransactionsOrchestrator.FindAccountPaymentTransactions(accountHashedId, fromDate, toDate, OwinWrapper.GetClaimValue(@"sub"));

            return View("PaymentDetails", viewModel);
        }
    }
}
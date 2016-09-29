using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{accountId}")]
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
        public async Task<ActionResult> Index(string accountId)
        {
            var transactionViewResult  = await _accountTransactionsOrchestrator.GetAccountTransactions(accountId, OwinWrapper.GetClaimValue(@"sub"));

            if (transactionViewResult.Account == null)
            {
                return RedirectToAction("Index", "AccessDenied");
            }

            transactionViewResult.Model.Data.HashedId = accountId;
            return View(transactionViewResult.Model);
        }

        [Route("Balance/{itemId}/Detail")]
        public async Task<ActionResult> Detail(string accountId, string itemId)
        {
            var transactionViewResult = await _accountTransactionsOrchestrator.GetAccounTransactionLineItem(accountId, itemId, OwinWrapper.GetClaimValue(@"sub"));

            if (transactionViewResult.Account == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (transactionViewResult.Model.LineItem == null)
            {
                return RedirectToAction("Index", "EmployerAccountTransactions", new {accountId});
            }
           
            return View(transactionViewResult.Model);
        }
    }
}
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Queries.GetTransferTransactionDetails;
using SFA.DAS.EmployerFinance.Web.Extensions;
using SFA.DAS.EmployerFinance.Web.Helpers;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountTransactionsController : BaseController
    {
        private readonly IAuthenticationService _owinWrapper;
        private readonly EmployerAccountTransactionsOrchestrator _accountTransactionsOrchestrator;



        public EmployerAccountTransactionsController(
            IAuthenticationService owinWrapper,
            EmployerAccountTransactionsOrchestrator accountTransactionsOrchestrator)
        : base(owinWrapper)
        {
            _owinWrapper = owinWrapper;
            _accountTransactionsOrchestrator = accountTransactionsOrchestrator;
        }

        [Route("finance/provider/summary")]
        [Route("balance/provider/summary")]
        public async Task<ActionResult> ProviderPaymentSummary(string hashedAccountId, long ukprn, DateTime fromDate, DateTime toDate)
        {
            var viewModel = await _accountTransactionsOrchestrator.GetProviderPaymentSummary(hashedAccountId, ukprn, fromDate, toDate, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(ControllerConstants.ProviderPaymentSummaryViewName, viewModel);
        }

        [Route("finance")]
        [Route("balance")]
        public ActionResult Index(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("finance"));
        }

        [Route("finance/downloadtransactions")]
        public ActionResult TransactionsDownload(string hashedAccountId)
        {
            return Redirect(Url.LegacyEasAccountAction("finance/downloadtransactions"));
        }

        [Route("finance/{year}/{month}")]
        [Route("balance/{year}/{month}")]
        public async Task<ActionResult> TransactionsView(string hashedAccountId, int year, int month)
        {
            var transactionViewResult = await _accountTransactionsOrchestrator.GetAccountTransactions(hashedAccountId, year, month, _owinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            if (transactionViewResult.Data.Account == null)
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.AccessDeniedControllerName);
            }

            transactionViewResult.Data.Model.Data.HashedAccountId = hashedAccountId;
            return View(transactionViewResult);
        }


        [Route("finance/levyDeclaration/details")]
        [Route("balance/levyDeclaration/details")]
        public async Task<ActionResult> LevyDeclarationDetail(string hashedAccountId, DateTime fromDate, DateTime toDate)
        {
            var viewModel = await _accountTransactionsOrchestrator.FindAccountLevyDeclarationTransactions(hashedAccountId, fromDate, toDate, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(ControllerConstants.LevyDeclarationDetailViewName, viewModel);
        }

        [Route("finance/course/standard/summary")]
        [Route("balance/course/standard/summary")]
        public async Task<ActionResult> CourseStandardPaymentSummary(string hashedAccountId, long ukprn, string courseName,
            int? courseLevel, DateTime fromDate, DateTime toDate)
        {
            return await CourseFrameworkPaymentSummary(hashedAccountId, ukprn, courseName, courseLevel, null, fromDate, toDate);
        }

        [Route("finance/course/framework/summary")]
        [Route("balance/course/framework/summary")]
        public async Task<ActionResult> CourseFrameworkPaymentSummary(string hashedAccountId, long ukprn, string courseName,
            int? courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate)
        {
            var viewModel = await _accountTransactionsOrchestrator.GetCoursePaymentSummary(
                hashedAccountId, ukprn, courseName, courseLevel, pathwayCode,
                fromDate, toDate, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(ControllerConstants.CoursePaymentSummaryViewName, viewModel);
        }

        [Route("finance/transfer/details")]
        [Route("balance/transfer/details")]
        public ActionResult TransferDetail(GetTransferTransactionDetailsQuery query)
        {
            return Redirect(Url.LegacyEasAccountAction($"finance/transfer/details{Request?.Url?.Query}"));
        }

    }
}
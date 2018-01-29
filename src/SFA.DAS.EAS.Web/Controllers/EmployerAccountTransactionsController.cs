using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountTransactionsController : BaseController
    {
        private readonly EmployerAccountTransactionsOrchestrator _accountTransactionsOrchestrator;
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;

        public EmployerAccountTransactionsController(IOwinWrapper owinWrapper, IFeatureToggleService featureToggle,
            IHashingService hashingService,
            IMediator mediator,
            EmployerAccountTransactionsOrchestrator accountTransactionsOrchestrator, IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage, ITransactionFormatterFactory transactionsFormatterFactory)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _mediator = mediator;
            _hashingService = hashingService;
            _accountTransactionsOrchestrator = accountTransactionsOrchestrator;
        }

        [Route("finance")]
        [Route("balance")]
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            var transactionViewResult = await _accountTransactionsOrchestrator.GetFinanceDashboardViewModel(hashedAccountId, 0, 0, OwinWrapper.GetClaimValue(ControllerConstants.SubClaimKeyName));

            if (transactionViewResult.Data.Account == null)
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.AccessDeniedControllerName);
            }

            return View(transactionViewResult);
        }

        [Route("finance/downloadtransactions")]
        [Route("balance/downloadtransactions")]
        public ActionResult TransactionsDownload(string hashedAccountId)
        {
            return View(new GetTransactionsDownloadRequestAndResponse
            {
                HashedId = hashedAccountId
            });
        }

        [Route("finance/downloadtransactionsdata")]
        public async Task<ActionResult> TransactionDownloadByDate(string hashedAccountId,
            GetTransactionsDownloadRequestAndResponse viewModel)
        {
            const string viewName = @"TransactionsDownload";

            if (!viewModel.Valid)
            {
                return View(viewName, viewModel);
            }

            var accountId = _hashingService.DecodeValue(hashedAccountId);

            viewModel.AccountId = accountId;
            viewModel.ExternalUserId = OwinWrapper.GetClaimValue(ControllerConstants.SubClaimKeyName);

            var task = _mediator.SendAsync(viewModel);

            var transactionsDownloadResultViewModel = await task;

            if (transactionsDownloadResultViewModel.IsUnauthorized)
            {
                return RedirectToAction(ControllerConstants.IndexActionName,
                    ControllerConstants.AccessDeniedControllerName);
            }

            if ((task.Exception != null && task.Exception.InnerExceptions.Any()) ||
                !transactionsDownloadResultViewModel.Valid ||
                transactionsDownloadResultViewModel.Transactions == null ||
                !transactionsDownloadResultViewModel.Transactions.Any())
            {
                return View(viewName, transactionsDownloadResultViewModel);
            }

            return File(transactionsDownloadResultViewModel.FileDate,
                transactionsDownloadResultViewModel.MimeType,
                $"esfaTransactions_{DateTime.Now:yyyyMMddHHmmss}.{transactionsDownloadResultViewModel.FileExtension}");
        }

        [Route("finance/{year}/{month}")]
        [Route("balance/{year}/{month}")]
        public async Task<ActionResult> TransactionsView(string hashedAccountId, int year, int month)
        {
            var transactionViewResult = await _accountTransactionsOrchestrator.GetAccountTransactions(hashedAccountId, year, month, OwinWrapper.GetClaimValue(ControllerConstants.SubClaimKeyName));

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
            var viewModel = await _accountTransactionsOrchestrator.FindAccountLevyDeclarationTransactions(hashedAccountId, fromDate, toDate, OwinWrapper.GetClaimValue(ControllerConstants.SubClaimKeyName));

            return View(ControllerConstants.LevyDeclarationDetailViewName, viewModel);
        }

        [Route("finance/provider/summary")]
        [Route("balance/provider/summary")]
        public async Task<ActionResult> ProviderPaymentSummary(string hashedAccountId, long ukprn, DateTime fromDate, DateTime toDate)
        {
            var viewModel = await _accountTransactionsOrchestrator.GetProviderPaymentSummary(hashedAccountId, ukprn, fromDate, toDate, OwinWrapper.GetClaimValue(ControllerConstants.SubClaimKeyName));

            return View(ControllerConstants.ProviderPaymentSummaryViewName, viewModel);
        }

        [Route("finance/course/standard/summary")]
        [Route("balance/course/standard/summary")]
        public async Task<ActionResult> CourseStandardPaymentSummary(string hashedAccountId, long ukprn, string courseName,
            int courseLevel, DateTime fromDate, DateTime toDate)
        {
            return await CourseFrameworkPaymentSummary(hashedAccountId, ukprn, courseName, courseLevel, null, fromDate, toDate);
        }

        [Route("finance/course/framework/summary")]
        [Route("balance/course/framework/summary")]
        public async Task<ActionResult> CourseFrameworkPaymentSummary(string hashedAccountId, long ukprn, string courseName, 
            int courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate)
        {
            var viewModel = await _accountTransactionsOrchestrator.GetCoursePaymentSummary(
                                                                        hashedAccountId, ukprn, courseName, courseLevel, pathwayCode,
                                                                        fromDate, toDate, OwinWrapper.GetClaimValue(ControllerConstants.SubClaimKeyName));

            return View(ControllerConstants.CoursePaymentSummaryViewName, viewModel);
        }
    }
}
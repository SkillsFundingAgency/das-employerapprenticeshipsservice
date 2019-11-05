﻿using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Queries.GetTransferTransactionDetails;
using SFA.DAS.EmployerFinance.Web.Helpers;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using SFA.DAS.Validation.Mvc;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.NLog.Logger;
using SFA.DAS.EmployerFinance.Web.Filters;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("accounts/{HashedAccountId}")]
    public class EmployerAccountTransactionsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        private readonly IAuthenticationService _owinWrapper;
        private readonly EmployerAccountTransactionsOrchestrator _accountTransactionsOrchestrator;

        public EmployerAccountTransactionsController(
            IAuthenticationService owinWrapper,
            EmployerAccountTransactionsOrchestrator accountTransactionsOrchestrator,
            IMapper mapper,
            IMediator mediator,
            ILog logger)
        : base(owinWrapper)
        {
            _owinWrapper = owinWrapper;
            _accountTransactionsOrchestrator = accountTransactionsOrchestrator;

            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
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
        public async Task<ActionResult> Index(GetAccountFinanceOverviewQuery query)
        {
            var viewModel = await _accountTransactionsOrchestrator.Index(query);

            if (viewModel.RedirectUrl != null)
                return Redirect(viewModel.RedirectUrl);

            return View(viewModel);
        }

        [HttpGet]
        [Route("finance/employer-guidance")]
        public async Task<ActionResult> EmployerGuidanceR02()
        {
            return View();
        }

        [ImportModelStateFromTempData]
        [Route("finance/downloadtransactions")]
        public ActionResult TransactionsDownload()
        {
            return View(new TransactionDownloadViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("finance/downloadtransactions")]
        public async Task<ActionResult> TransactionsDownload(TransactionDownloadViewModel model)
        {
            var response = await _mediator.SendAsync(model.GetTransactionsDownloadQuery);
            return File(response.FileData, response.MimeType, $"esfaTransactions_{DateTime.Now:yyyyMMddHHmmss}.{response.FileExtension}");
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

            //TODO: Remove this code once we have finished investigations into CON-111
            var payments = transactionViewResult.Data?.Model?.Data?.TransactionLines?.Where(t =>
                t.TransactionType == TransactionItemType.Payment);

            if (payments != null)
            {
                foreach (var payment in payments)
                {
                    if (!(payment is PaymentTransactionLine))
                    {
                        _logger.Warn(
                            $"Invalid payment transaction detected. Account ID: {payment.AccountId}, Date created: {payment.DateCreated.ToLongTimeString()}, transaction date: {payment.TransactionDate}");
                    }
                }
            }
            // END OF CODE FOR CON-111

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
            var orchestratorResponse = await _accountTransactionsOrchestrator.GetCoursePaymentSummary(
                hashedAccountId, ukprn, courseName, courseLevel, pathwayCode,
                fromDate, toDate, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return View(ControllerConstants.CoursePaymentSummaryViewName, orchestratorResponse.Data);
        }

        [Route("finance/transfer/details")]
        [Route("balance/transfer/details")]
        public async Task<ActionResult> TransferDetail(GetTransferTransactionDetailsQuery query)
        {
            var response = await _mediator.SendAsync(query);

            var model = _mapper.Map<TransferTransactionDetailsViewModel>(response);
            return View(ControllerConstants.TransferDetailsViewName, model);
        }

    }
}
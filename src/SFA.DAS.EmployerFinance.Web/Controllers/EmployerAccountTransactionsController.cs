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
        private readonly EmployerAccountTransactionsOrchestrator _accountTransactionsOrchestrator;
        //private readonly IMapper _mapper;
        //private readonly IMediator _mediator;

        //public EmployerAccountTransactionsController(IAuthenticationService owinWrapper, IAuthorizationService authorization,
        //IHashingService hashingService,
        //IMediator mediator,
        //EmployerAccountTransactionsOrchestrator accountTransactionsOrchestrator, IMultiVariantTestingService multiVariantTestingService,
        //ICookieStorageService<FlashMessageViewModel> flashMessage, ITransactionFormatterFactory transactionsFormatterFactory,
        //IMapper mapper)
        //: base(owinWrapper, multiVariantTestingService, flashMessage)
        //{
        //    _mediator = mediator;
        //    _accountTransactionsOrchestrator = accountTransactionsOrchestrator;
        //    _mapper = mapper;
        //}

        public EmployerAccountTransactionsController(IAuthenticationService owinWrapper,
        EmployerAccountTransactionsOrchestrator accountTransactionsOrchestrator)
        : base(owinWrapper)
        {
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
        public ActionResult TransactionsView(string hashedAccountId, int year, int month)
        {
            return Redirect(Url.LegacyEasAccountAction($"finance/{year}/{month}"));
        }

        [Route("finance/levyDeclaration/details")]
        [Route("balance/levyDeclaration/details")]
        public ActionResult LevyDeclarationDetail(string hashedAccountId, DateTime fromDate, DateTime toDate)
        {
           return Redirect(Url.LegacyEasAccountAction($"finance/levyDeclaration/details{Request?.Url?.Query}"));
        }


        [Route("finance/course/standard/summary")]
        [Route("balance/course/standard/summary")]
        public ActionResult CourseStandardPaymentSummary(string hashedAccountId, long ukprn, string courseName,
            int? courseLevel, DateTime fromDate, DateTime toDate)
        {
            return Redirect(Url.LegacyEasAccountAction($"finance/course/standard/summary{Request?.Url?.Query}"));
        }

        [Route("finance/course/framework/summary")]
        [Route("balance/course/framework/summary")]
        public ActionResult CourseFrameworkPaymentSummary(string hashedAccountId, long ukprn, string courseName,
            int? courseLevel, int? pathwayCode, DateTime fromDate, DateTime toDate)
        {
            return Redirect(Url.LegacyEasAccountAction($"finance/course/framework/summary{Request?.Url?.Query}"));
        }

        [Route("finance/transfer/details")]
        [Route("balance/transfer/details")]
        public ActionResult TransferDetail(GetTransferTransactionDetailsQuery query)
        {
            return Redirect(Url.LegacyEasAccountAction($"finance/transfer/details{Request?.Url?.Query}"));
        }
    }
}
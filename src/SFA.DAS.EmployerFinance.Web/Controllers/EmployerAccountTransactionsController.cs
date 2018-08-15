using SFA.DAS.Authentication;
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

       
        public ActionResult Index()
        {
            // Dummy method for replacement by future merge
            throw new NotImplementedException();
        }
    }
}
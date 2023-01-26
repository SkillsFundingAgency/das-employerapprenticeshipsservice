using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerFinance.Web.Orchestrators;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [DasAuthorize(EmployerUserRole.Any)]
    [RoutePrefix("accounts/{HashedAccountId}")] 
    public class TransfersController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly TransfersOrchestrator _transfersOrchestrator;

        public TransfersController(TransfersOrchestrator transfersOrchestrator)
        {
            _transfersOrchestrator = transfersOrchestrator;
        }

        [HttpGet]
        [Route("transfers")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index(string hashedAccountId)
        {
            var viewModel = await _transfersOrchestrator.GetIndexViewModel(hashedAccountId);

            return View(viewModel);
        }

        [DasAuthorize("EmployerFeature.FinanceDetails")]
        [HttpGet]
        [Route("transfers/financial-breakdown")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> FinancialBreakdown(string hashedAccountId)
        {
            var viewModel = await _transfersOrchestrator.GetFinancialBreakdownViewModel(hashedAccountId);
            return View(viewModel);
        }
    }
}
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerCommitmentsController : BaseController
    {
        private readonly EmployerCommitmentsOrchestrator _employerCommitmentsOrchestrator;
        private readonly IOwinWrapper _owinWrapper;

        public EmployerCommitmentsController(EmployerCommitmentsOrchestrator employerCommitmentsOrchestrator, IOwinWrapper owinWrapper)
        {
            if (employerCommitmentsOrchestrator == null)
                throw new ArgumentNullException(nameof(employerCommitmentsOrchestrator));
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            _employerCommitmentsOrchestrator = employerCommitmentsOrchestrator;
            _owinWrapper = owinWrapper;
        }

        [HttpGet]
        public async Task<ActionResult> Index(long accountid)
        {
            var model = await _employerCommitmentsOrchestrator.GetAll(accountid);

            return View(model);
        }

        [HttpGet]
        public async Task<ActionResult> Create(long accountId)
        {
            var model = await _employerCommitmentsOrchestrator.GetNew(accountId, _owinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateCommitmentViewModel commitment)
        {
            await _employerCommitmentsOrchestrator.Create(commitment, _owinWrapper.GetClaimValue(@"sub"));

            return RedirectToAction("Index", new {accountId = commitment.AccountId});
        }

        [HttpGet]
        public async Task<ActionResult> Details(long accountId, long commitmentId)
        {
            return RedirectToAction("Index", new { accountId = accountId });
        }
    }
}
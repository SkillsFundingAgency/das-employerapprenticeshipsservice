using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedaccountId}")]
    public class EmployerCommitmentsController : BaseController
    {
        private readonly EmployerCommitmentsOrchestrator _employerCommitmentsOrchestrator;

        public EmployerCommitmentsController(EmployerCommitmentsOrchestrator employerCommitmentsOrchestrator, IOwinWrapper owinWrapper, 
            IFeatureToggle featureToggle, IUserWhiteList userWhiteList) 
            : base(owinWrapper,featureToggle, userWhiteList)
        {
            if (employerCommitmentsOrchestrator == null)
                throw new ArgumentNullException(nameof(employerCommitmentsOrchestrator));
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            _employerCommitmentsOrchestrator = employerCommitmentsOrchestrator;
        }

        [HttpGet]
        [Route("Commitments")]
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            var model = await _employerCommitmentsOrchestrator.GetAll(hashedAccountId);

            return View(model);
        }

        [HttpGet]
        [Route("Commitments/Inform")]
        public ActionResult Inform(string hashedAccountId)
        {
            var model = new CommitmentInformViewModel
            {
                HashedAccountId = hashedAccountId
            };

            return View(model);
        }

        [HttpGet]
        [Route("Commitments/Create/LegalEntity")]
        public async Task<ActionResult> SelectLegalEntity(string hashedAccountId)
        {
            var model = await _employerCommitmentsOrchestrator.GetLegalEntities(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [Route("Commitments/Create/LegalEntity")]
        public ActionResult SetLegalEntity(CreateCommitmentModel commitment)
        {
            return RedirectToAction("SelectProvider", commitment);
        }

        [HttpGet]
        [Route("Commitments/Create/Provider")]
        public async Task<ActionResult> SelectProvider(string hashedAccountId, long legalEntityId, string legalEntityName)
        {
            var model = await _employerCommitmentsOrchestrator.GetProviders(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            model.Data.Commitment.LegalEntityId = legalEntityId;
            model.Data.Commitment.LegalEntityName = legalEntityName;

            return View(model);
        }

        [HttpPost]
        [Route("Commitments/Create/Provider")]
        public ActionResult SetProvider(CreateCommitmentModel commitment)
        {
            return RedirectToAction("SelectName", commitment);
        }

        [HttpGet]
        [Route("Commitments/Create/Name")]
        public async Task<ActionResult> SelectName(CreateCommitmentModel commitment)
        {
            var model = await _employerCommitmentsOrchestrator.CreateSummary(commitment, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [Route("Commitments/Create")]
        public async Task<ActionResult> CreateCommitment(CreateCommitmentViewModel commitment)
        {
            await _employerCommitmentsOrchestrator.Create(commitment, OwinWrapper.GetClaimValue(@"sub"));

            return RedirectToAction("Index");
        }

        [HttpGet]
        [Route("Commitments/{hashedCommitmentId}/Details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedCommitmentId)
        {
            var model = await _employerCommitmentsOrchestrator.Get(hashedAccountId, hashedCommitmentId);

            return View(model);
        }

        [HttpGet]
        [Route("Commitments/{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Details")]
        public async Task<ActionResult> ApprenticeshipDetails(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var model = await _employerCommitmentsOrchestrator.GetApprenticeship(hashedAccountId, hashedCommitmentId, hashedApprenticeshipId);

            return View(model);
        }

        [HttpPost]
        [Route("UpdateApprenticeship")]
        public ActionResult UpdateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            return RedirectToAction("Index", new { hashedAccountId = apprenticeship.HashedAccountId, hashedCommitmentId = apprenticeship.HashedCommitmentId });
        }

        [HttpGet]
        [Route("Commitments/{hashedCommitmentId}/Submit")]
        public async Task<ActionResult> SubmitCommitmentEntry(string hashedAccountId, string hashedCommitmentId)
        {
            var commitment = await _employerCommitmentsOrchestrator.Get(hashedAccountId, hashedCommitmentId);
            
            var model = new SubmitCommitmentViewModel
            {
                SubmitCommitmentModel = new SubmitCommitmentModel
                {
                    HashedAccountId = hashedAccountId,
                    HashedCommitmentId = hashedCommitmentId
                },
                Commitment = commitment
            };

            return View(model);
        }

        [HttpPost]
        [Route("Commitments/{hashedCommitmentId}/Submit")]
        public async Task<ActionResult> SubmitCommitment(SubmitCommitmentModel model)
        {
            await _employerCommitmentsOrchestrator.SubmitCommitment(model.HashedAccountId, model.HashedCommitmentId, model.Message);

            return RedirectToAction("Index", new { accountid = model.HashedAccountId });
        }

        [HttpPost]
        [Route("Commitments/{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Approve")]
        public async Task<ActionResult> ApproveApprenticeship([System.Web.Http.FromUri]ApproveApprenticeshipModel model)
        {
            await _employerCommitmentsOrchestrator.ApproveApprenticeship(model);

            return RedirectToAction("Details", new { hashedAccountId = model.HashedAccountId, hashedCommitmentId = model.HashedCommitmentId });
        }

        [HttpPost]
        [Route("Commitments/{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Pause")]
        public async Task<ActionResult> PauseApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            await _employerCommitmentsOrchestrator.PauseApprenticeship(hashedAccountId, hashedCommitmentId, hashedApprenticeshipId);

            return RedirectToAction("Details", new { hashedAccountId = hashedAccountId, hashedCommitmentId = hashedCommitmentId });
        }

        [HttpPost]
        [Route("Commitments/{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Resume")]
        public async Task<ActionResult> ResumeApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            await _employerCommitmentsOrchestrator.ResumeApprenticeship(hashedAccountId, hashedCommitmentId, hashedApprenticeshipId);

            return RedirectToAction("Details", new { hashedAccountId = hashedAccountId, hashedCommitmentId = hashedCommitmentId });
        }

        [HttpGet]
        [Route("Commitments/{hashedCommitmentId}/Apprenticeships/Create")]
        public async Task<ActionResult> CreateApprenticeshipEntry(string hashedAccountId, string hashedCommitmentId)
        {
            var model = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(hashedAccountId, hashedCommitmentId);

            return View(model);
        }

        [HttpPost]
        [Route("Commitments/{hashedCommitmentId}/Apprenticeships/Create")]
        public async Task<ActionResult> CreateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            await _employerCommitmentsOrchestrator.CreateApprenticeship(apprenticeship);

            return RedirectToAction("Details", new { hashedAccountId = apprenticeship.HashedAccountId, hashedCommitmentId = apprenticeship.HashedCommitmentId });
        }
    }
}
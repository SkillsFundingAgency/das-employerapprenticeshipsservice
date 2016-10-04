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
        [Route("Commitments/{commitmentId}/Details")]
        public async Task<ActionResult> Details(string hashedAccountId, long commitmentId)
        {
            var model = await _employerCommitmentsOrchestrator.Get(hashedAccountId, commitmentId);

            return View(model);
        }

        [HttpGet]
        [Route("Commitments/{commitmentId}/Apprenticeships/{apprenticeshipId}/Details")]
        public async Task<ActionResult> ApprenticeshipDetails(string hashedAccountId, long commitmentId, long apprenticeshipId)
        {
            var model = await _employerCommitmentsOrchestrator.GetApprenticeship(hashedAccountId, commitmentId, apprenticeshipId);

            return View(model);
        }

        [HttpPost]
        [Route("UpdateApprenticeship")]
        public ActionResult UpdateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            return RedirectToAction("Index", new { hashedAccountId = apprenticeship.HashedAccountId, commitmentId = apprenticeship.CommitmentId });
        }

        [HttpGet]
        [Route("Commitments/{commitmentId}/Submit")]
        public async Task<ActionResult> SubmitCommitmentEntry(string hashedAccountId, long commitmentId)
        {
            var commitment = await _employerCommitmentsOrchestrator.Get(hashedAccountId, commitmentId);
            
            var model = new SubmitCommitmentViewModel
            {
                SubmitCommitmentModel = new SubmitCommitmentModel
                {
                    HashedAccountId = hashedAccountId,
                    CommitmentId = commitmentId
                },
                Commitment = commitment.Commitment
            };

            return View(model);
        }

        [HttpPost]
        [Route("Commitments/{commitmentId}/Submit")]
        public async Task<ActionResult> SubmitCommitment(SubmitCommitmentModel model)
        {
            await _employerCommitmentsOrchestrator.SubmitCommitment(model.HashedAccountId, model.CommitmentId, model.Message);

            return RedirectToAction("Index", new { accountid = model.HashedAccountId, commitmentId = model.CommitmentId });
        }

        [HttpPost]
        [Route("Commitments/{commitmentId}/Apprenticeships/{apprenticeshipId}/Approve")]
        public async Task<ActionResult> ApproveApprenticeship([System.Web.Http.FromUri]ApproveApprenticeshipModel model)
        {
            await _employerCommitmentsOrchestrator.ApproveApprenticeship(model);

            return RedirectToAction("Details", new { hashedAccountId = model.HashedAccountId, commitmentId = model.CommitmentId });
        }

        [HttpPost]
        [Route("Commitments/{commitmentId}/Apprenticeships/{apprenticeshipId}/Pause")]
        public async Task<ActionResult> PauseApprenticeship(string hashedAccountId, long commitmentId, long apprenticeshipId)
        {
            await _employerCommitmentsOrchestrator.PauseApprenticeship(hashedAccountId, commitmentId, apprenticeshipId);

            return RedirectToAction("Details", new { hashedAccountId = hashedAccountId, commitmentId = commitmentId });
        }

        [HttpPost]
        [Route("Commitments/{commitmentId}/Apprenticeships/{apprenticeshipId}/Resume")]
        public async Task<ActionResult> ResumeApprenticeship(string hashedAccountId, long commitmentId, long apprenticeshipId)
        {
            await _employerCommitmentsOrchestrator.ResumeApprenticeship(hashedAccountId, commitmentId, apprenticeshipId);

            return RedirectToAction("Details", new { hashedAccountId = hashedAccountId, commitmentId = commitmentId });
        }

        [HttpGet]
        [Route("Commitments/{commitmentId}/Apprenticeships/Create")]
        public async Task<ActionResult> CreateApprenticeshipEntry(string hashedAccountId, long commitmentId)
        {
            var model = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(hashedAccountId, commitmentId);

            return View(model);
        }

        [HttpPost]
        [Route("Commitments/{commitmentId}/Apprenticeships/Create")]
        public async Task<ActionResult> CreateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            await _employerCommitmentsOrchestrator.CreateApprenticeship(apprenticeship);

            return RedirectToAction("Details", new { hashedAccountId = apprenticeship.HashedAccountId, commitmentId = apprenticeship.CommitmentId });
        }
    }
}
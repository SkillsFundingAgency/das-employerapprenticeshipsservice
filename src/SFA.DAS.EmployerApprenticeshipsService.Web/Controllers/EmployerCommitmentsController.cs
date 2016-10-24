using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Application;
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
        [ValidateAntiForgeryToken]
        [Route("Commitments/Create/LegalEntity")]
        public ActionResult SetLegalEntity(CreateCommitmentModel commitment)
        {
            return RedirectToAction("SelectProvider", new { hashedAccountId = commitment.HashedAccountId, legalEntityCode = commitment.LegalEntityCode});
        }

        [HttpGet]
        //[Route("Commitments/Create/Provider")]
        public ActionResult SelectProvider(string hashedAccountId, string legalEntityCode)
        {
            var model = new CreateCommitmentViewModel
            {
                HashedAccountId = hashedAccountId,
                LegalEntityCode = legalEntityCode
            };

            return View(model);
        }

        [HttpPost]
        //[Route("Commitments/Create/Provider")]
        public async Task<ActionResult> ConfirmProvider(CreateCommitmentModel commitment)
        {
            var providers = await _employerCommitmentsOrchestrator.FindProviders(commitment.UkPrn);

            var model = new ConfirmProviderView
            {
                HashedAccountId = commitment.HashedAccountId,
                LegalEntityCode = commitment.LegalEntityCode,
                UkPrn = commitment.UkPrn,
                Providers = providers
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
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
        [ValidateAntiForgeryToken]
        [Route("Commitments/{hashedCommitmentId}/Submit")]
        public async Task<ActionResult> SubmitCommitment(SubmitCommitmentModel model)
        {
            await _employerCommitmentsOrchestrator.SubmitCommitment(model.HashedAccountId, model.HashedCommitmentId, model.Message);

            return RedirectToAction("Index", new { accountid = model.HashedAccountId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Commitments/{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Approve")]
        public async Task<ActionResult> ApproveApprenticeship([System.Web.Http.FromUri]ApproveApprenticeshipModel model)
        {
            await _employerCommitmentsOrchestrator.ApproveApprenticeship(model);

            return RedirectToAction("Details", new { hashedAccountId = model.HashedAccountId, hashedCommitmentId = model.HashedCommitmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Commitments/{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Pause")]
        public async Task<ActionResult> PauseApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            await _employerCommitmentsOrchestrator.PauseApprenticeship(hashedAccountId, hashedCommitmentId, hashedApprenticeshipId);

            return RedirectToAction("Details", new { hashedAccountId = hashedAccountId, hashedCommitmentId = hashedCommitmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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
            try
            {
                await _employerCommitmentsOrchestrator.CreateApprenticeship(apprenticeship);
            }
            catch (InvalidRequestException ex)
            {
                var model = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(apprenticeship.HashedAccountId, apprenticeship.HashedCommitmentId);
                model.Apprenticeship = apprenticeship;

                foreach (var error in ex.ErrorMessages)
                {
                    ModelState.AddModelError(error.Key, error.Value);
                }

                return View("CreateApprenticeshipEntry", model);
            }

            return RedirectToAction("Details", new { hashedAccountId = apprenticeship.HashedAccountId, hashedCommitmentId = apprenticeship.HashedCommitmentId });
        }
    }
}
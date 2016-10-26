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
    [RoutePrefix("accounts/{hashedaccountId}/Commitments")]
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
        [Route("Home")]
        public ActionResult Index(string hashedAccountId)
        {
            ViewBag.HashedAccountId = hashedAccountId;

            return View();
        }

        [HttpGet]
        [Route("Cohorts")]
        public async Task<ActionResult> Cohorts(string hashedAccountId)
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
            var legalEntities = await _employerCommitmentsOrchestrator.GetLegalEntities(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            ViewBag.LegalEntities = legalEntities.Data;

            return View(new SelectLegalEntityViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Commitments/Create/LegalEntity")]
        public async Task<ActionResult> SetLegalEntity(string hashedAccountId, SelectLegalEntityViewModel selectedLegalEntity)
        {
            if (!ModelState.IsValid)
            {
                var legalEntities = await _employerCommitmentsOrchestrator.GetLegalEntities(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
                ViewBag.LegalEntities = legalEntities.Data;

                return View("SelectLegalEntity", selectedLegalEntity);
            }

            return RedirectToAction("SelectProvider", selectedLegalEntity);
        }

        [HttpGet]
        [Route("Commitments/Create/Provider")]
        public async Task<ActionResult> SelectProvider(string hashedAccountId, string legalEntityCode)
        {
            var providers = await _employerCommitmentsOrchestrator.GetProviders(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            ViewBag.Providers = providers.Data;

            return View(new SelectProviderViewModel { LegalEntityCode = legalEntityCode });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Commitments/Create/Provider")]
        public async Task<ActionResult> SetProvider(string hashedAccountId, [System.Web.Http.FromUri]SelectProviderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var providers = await _employerCommitmentsOrchestrator.GetProviders(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));
                ViewBag.Providers = providers.Data;

                return View("SelectProvider", viewModel);
            }

            return RedirectToAction("SelectName", viewModel);
        }

        [HttpGet]
        [Route("Commitments/Create/Name")]
        public async Task<ActionResult> SelectName(string hashedAccountId, string legalEntityCode, string providerId)
        {
            var model = await _employerCommitmentsOrchestrator.CreateSummary(hashedAccountId, legalEntityCode, providerId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Commitments/Create")]
        public async Task<ActionResult> CreateCommitment(CreateCommitmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("SelectName", viewModel);
            }

            await _employerCommitmentsOrchestrator.Create(viewModel, OwinWrapper.GetClaimValue(@"sub"));

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

            ViewBag.ApprenticeshipProducts = model.Standards;

            return View(model.Apprenticeship);
        }

        [HttpPost]
        [Route("Commitments/{hashedCommitmentId}/Apprenticeships/Create")]
        public async Task<ActionResult> CreateApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return await RedisplayCreateApprenticeshipView(apprenticeship);
                }

                await _employerCommitmentsOrchestrator.CreateApprenticeship(apprenticeship);
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex);

                return await RedisplayCreateApprenticeshipView(apprenticeship);
            }

            return RedirectToAction("Details", new { hashedAccountId = apprenticeship.HashedAccountId, hashedCommitmentId = apprenticeship.HashedCommitmentId });
        }

        private void AddErrorsToModelState(InvalidRequestException ex)
        {
            foreach (var error in ex.ErrorMessages)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        private async Task<ActionResult> RedisplayCreateApprenticeshipView(ApprenticeshipViewModel apprenticeship)
        {
            var model = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(apprenticeship.HashedAccountId, apprenticeship.HashedCommitmentId);
            model.Apprenticeship = apprenticeship;
            ViewBag.ApprenticeshipProducts = model.Standards;

            return View("CreateApprenticeshipEntry", model.Apprenticeship);
        }
    }
}
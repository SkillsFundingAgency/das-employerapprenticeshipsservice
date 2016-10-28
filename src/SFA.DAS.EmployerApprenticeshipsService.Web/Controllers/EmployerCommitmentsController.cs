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
            : base(owinWrapper, featureToggle, userWhiteList)
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
        [Route("Inform")]
        public ActionResult Inform(string hashedAccountId)
        {
            var model = new CommitmentInformViewModel
            {
                HashedAccountId = hashedAccountId
            };

            return View(model);
        }

        [HttpGet]
        [Route("Create/LegalEntity")]
        public async Task<ActionResult> SelectLegalEntity(string hashedAccountId)
        {
            var legalEntities = await _employerCommitmentsOrchestrator.GetLegalEntities(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            ViewBag.LegalEntities = legalEntities.Data;

            return View(new SelectLegalEntityViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create/LegalEntity")]
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
        [Route("Create/Provider")]
        public ActionResult SearchProvider(string hashedAccountId, string legalEntityCode)
        {
            return View(new SelectProviderViewModel { LegalEntityCode = legalEntityCode });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create/Provider")]
        public async Task<ActionResult> SelectProvider(string hashedAccountId, [System.Web.Http.FromUri] SelectProviderViewModel viewModel)
        {
            var providerId = int.Parse(viewModel.ProviderId);

            var providers = await _employerCommitmentsOrchestrator.GetProvider(providerId);

            return View(new ConfirmProviderView
            {
                HashedAccountId = hashedAccountId,
                LegalEntityCode = viewModel.LegalEntityCode,
                ProviderId = providerId,
                Providers = providers
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create/ConfirmProvider")]
        public ActionResult ConfirmProvider(string hashedAccountId, [System.Web.Http.FromUri]SelectProviderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = new SelectProviderViewModel
                {
                    LegalEntityCode = viewModel.LegalEntityCode
                };

                if (string.IsNullOrWhiteSpace(viewModel.ProviderId))
                {
                    return RedirectToAction("SearchProvider", model);
                }

                return View("SearchProvider", model);
            }

            return RedirectToAction("ChoosePath", new {hashedAccountId = hashedAccountId, legalEntityCode = viewModel.LegalEntityCode, providerId = viewModel.ProviderId });
        }

        [HttpGet]
        [Route("Create/ChoosePath")]
        public async Task<ActionResult> ChoosePath(string hashedAccountId, string legalEntityCode, string legalEntityName, string providerId, string providerName)
        {
            var model = await _employerCommitmentsOrchestrator.CreateSummary(hashedAccountId, legalEntityCode, providerId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<ActionResult> CreateCommitment(CreateCommitmentViewModel viewModel, string selectedRoute)
        {
            viewModel.Name = CreateReference(); // TODO: LWA - Name needs to be deleted

            var hashedCommitmentId = await _employerCommitmentsOrchestrator.Create(viewModel, OwinWrapper.GetClaimValue(@"sub"));

            return RedirectToAction("SubmitCommitmentEntry", new { hashedCommitmentId = hashedCommitmentId });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedCommitmentId)
        {
            var model = await _employerCommitmentsOrchestrator.Get(hashedAccountId, hashedCommitmentId);

            return View(model);
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/FinishedEditing")]
        public ActionResult FinishedEditing(string hashedAccountId)
        {
            return View();
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/FinishedEditing")]
        public ActionResult FinishedEditingChoice(string hashedAccountId)
        {
            return RedirectToAction("SubmitCommitmentEntry");
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Details")]
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
        [Route("{hashedCommitmentId}/Submit")]
        public ActionResult SubmitCommitmentEntry(string hashedAccountId)
        {
            // TODO: LWA Implement 
            //var commitment = await _employerCommitmentsOrchestrator.Get(hashedAccountId, hashedCommitmentId);
            
            var model = new SubmitCommitmentViewModel
            {
                SubmitCommitmentModel = new SubmitCommitmentModel
                {
                    HashedAccountId = hashedAccountId,
                    //HashedCommitmentId = hashedCommitmentId
                },
               // Commitment = commitment
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Submit")]
        public ActionResult SubmitCommitment(SubmitCommitmentModel model)
        {
            // TODO: LWA Implement
            //await _employerCommitmentsOrchestrator.SubmitCommitment(model.HashedAccountId, model.HashedCommitmentId, model.Message);

            return RedirectToAction("Acknowledgement");
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        public ActionResult Acknowledgement(string hashedAccountId)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Approve")]
        public async Task<ActionResult> ApproveApprenticeship([System.Web.Http.FromUri]ApproveApprenticeshipModel model)
        {
            await _employerCommitmentsOrchestrator.ApproveApprenticeship(model);

            return RedirectToAction("Details", new { hashedAccountId = model.HashedAccountId, hashedCommitmentId = model.HashedCommitmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Pause")]
        public async Task<ActionResult> PauseApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            await _employerCommitmentsOrchestrator.PauseApprenticeship(hashedAccountId, hashedCommitmentId, hashedApprenticeshipId);

            return RedirectToAction("Details", new { hashedAccountId = hashedAccountId, hashedCommitmentId = hashedCommitmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Resume")]
        public async Task<ActionResult> ResumeApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            await _employerCommitmentsOrchestrator.ResumeApprenticeship(hashedAccountId, hashedCommitmentId, hashedApprenticeshipId);

            return RedirectToAction("Details", new { hashedAccountId = hashedAccountId, hashedCommitmentId = hashedCommitmentId });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Apprenticeships/Create")]
        public async Task<ActionResult> CreateApprenticeshipEntry(string hashedAccountId, string hashedCommitmentId)
        {
            var model = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(hashedAccountId, hashedCommitmentId);

            ViewBag.ApprenticeshipProducts = model.Standards;

            return View(model.Apprenticeship);
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/Apprenticeships/Create")]
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

        private static string CreateReference()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }
    }
}
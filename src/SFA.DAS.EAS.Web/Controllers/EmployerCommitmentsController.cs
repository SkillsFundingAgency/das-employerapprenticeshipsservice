using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedaccountId}/apprentices")]
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
        public async Task<ActionResult> SelectLegalEntity(string hashedAccountId, string cohortRef = "")
        {
            var legalEntities = await _employerCommitmentsOrchestrator.GetLegalEntities(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            ViewBag.LegalEntities = legalEntities.Data;

            if (string.IsNullOrWhiteSpace(cohortRef))
                cohortRef = CreateReference();

            return View(new SelectLegalEntityViewModel
            {
                CohortRef = cohortRef
            });
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

            return RedirectToAction("SearchProvider", selectedLegalEntity);
        }

        [HttpGet]
        [Route("Create/Provider")]
        public ActionResult SearchProvider(string hashedAccountId, string legalEntityCode, string cohortRef)
        {
            return View(new SelectProviderViewModel { LegalEntityCode = legalEntityCode, CohortRef = cohortRef });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create/Provider")]
        public async Task<ActionResult> SelectProvider(string hashedAccountId, [System.Web.Http.FromUri] SelectProviderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View("SearchProvider", new SelectProviderViewModel { LegalEntityCode = viewModel.LegalEntityCode, CohortRef = viewModel.CohortRef });
            }

            var providerId = int.Parse(viewModel.ProviderId);

            // The api returns a list but there should only ever be one per ukprn.
            var providers = await _employerCommitmentsOrchestrator.GetProvider(providerId);

            return View(new ConfirmProviderView
            {
                HashedAccountId = hashedAccountId,
                LegalEntityCode = viewModel.LegalEntityCode,
                ProviderId = providerId,
                Providers = providers,
                CohortRef = viewModel.CohortRef
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create/ConfirmProvider")]
        public async Task<ActionResult> ConfirmProvider(string hashedAccountId, [System.Web.Http.FromUri]ConfirmProviderView viewModel)
        {
            if (!ModelState.IsValid)
            {
                if (viewModel.Confirmation == null)
                {
                    viewModel.Providers = await _employerCommitmentsOrchestrator.GetProvider(viewModel.ProviderId);

                    return View("SelectProvider", viewModel);
                }
            }

            if (!viewModel.Confirmation.Value)
            {
                return RedirectToAction("SearchProvider", new SelectProviderViewModel { LegalEntityCode = viewModel.LegalEntityCode, CohortRef = viewModel.CohortRef });
            }

            return RedirectToAction("ChoosePath", new { hashedAccountId = hashedAccountId, legalEntityCode = viewModel.LegalEntityCode, providerId = viewModel.ProviderId, cohortRef = viewModel.CohortRef });
        }

        [HttpGet]
        [Route("Create/ChoosePath")]
        public async Task<ActionResult> ChoosePath(string hashedAccountId, string legalEntityCode, string legalEntityName, string providerId, string providerName, string cohortRef)
        {
            var model = await _employerCommitmentsOrchestrator.CreateSummary(hashedAccountId, legalEntityCode, providerId, OwinWrapper.GetClaimValue(@"sub"));

            model.Data.CohortRef = cohortRef;

            return View(model.Data);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<ActionResult> CreateCommitment(CreateCommitmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await _employerCommitmentsOrchestrator.CreateSummary(viewModel.HashedAccountId, viewModel.LegalEntityCode, viewModel.ProviderId.ToString(), OwinWrapper.GetClaimValue(@"sub"));

                model.Data.CohortRef = viewModel.CohortRef;

                return View("ChoosePath", model.Data);
            }

            if (viewModel.SelectedRoute == "employer")
            {
                var hashedCommitmentId = await _employerCommitmentsOrchestrator.CreateEmployerAssignedCommitment(viewModel);

                return RedirectToAction("Details", new { hashedCommitmentId = hashedCommitmentId });
            }

            return RedirectToAction("SubmitNewCommitment", new { hashedAccountId = viewModel.HashedAccountId, legalEntityCode = viewModel.LegalEntityCode, legalEntityName = viewModel.LegalEntityName, providerId = viewModel.ProviderId, providerName = viewModel.ProviderName, cohortRef = viewModel.CohortRef });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedCommitmentId)
        {
            var model = await _employerCommitmentsOrchestrator.Get(hashedAccountId, hashedCommitmentId);

            ViewBag.HashedAccountId = hashedAccountId;

            return View(model);
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Finished")]
        public async Task<ActionResult> FinishedEditing(string hashedAccountId, string hashedCommitmentId)
        {
            var viewmodel = await _employerCommitmentsOrchestrator.GetFinishEditingViewModel(hashedAccountId, hashedCommitmentId);

            return View(viewmodel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Finished")]
        public ActionResult FinishedEditing(FinishEditingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(viewModel);
            }

            // TODO: Refactor out these magic strings
            if (viewModel.SaveOrSend == "save-no-send")
            {
                return RedirectToAction("Cohorts", new { hashedAccountId = viewModel.HashedAccountId });
            }

            return RedirectToAction("SubmitExistingCommitment", new { hashedAccountId = viewModel.HashedAccountId, hashedCommitmentId = viewModel.HashedCommitmentId, saveOrSend = viewModel.SaveOrSend });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Edit")]
        public async Task<ActionResult> EditApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var model = await _employerCommitmentsOrchestrator.GetApprenticeship(hashedAccountId, hashedCommitmentId, hashedApprenticeshipId);

            ViewBag.ApprenticeshipProducts = model.ApprenticeshipProgrammes;

            return View("EditApprenticeshipEntry", model.Apprenticeship);
        }

        [HttpGet]
        [Route("Submit")]
        public ActionResult SubmitNewCommitment(string hashedAccountId, string legalEntityCode, string legalEntityName, string providerId, string providerName, string cohortRef, string saveOrSend)
        {
            var model = new SubmitCommitmentViewModel
            {
                HashedAccountId = hashedAccountId,
                LegalEntityCode = legalEntityCode,
                LegalEntityName = legalEntityName,
                ProviderId = long.Parse(providerId),
                ProviderName = providerName,
                CohortRef = cohortRef,
                SaveOrSend = saveOrSend
            };

            return View("SubmitCommitmentEntry", model);
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Submit")]
        public async Task<ActionResult> SubmitExistingCommitment(string hashedAccountId, string hashedCommitmentId, string saveOrSend)
        {
            // TODO: Should this be a different Orchestrator call?
            var commitment = await _employerCommitmentsOrchestrator.Get(hashedAccountId, hashedCommitmentId);

            var model = new SubmitCommitmentViewModel
            {
                HashedAccountId = hashedAccountId,
                HashedCommitmentId = hashedCommitmentId,
                ProviderName = commitment.ProviderName,
                SaveOrSend = saveOrSend
            };

            return View("SubmitCommitmentEntry", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Submit")]
        public async Task<ActionResult> SubmitExistingCommitmentEntry(SubmitCommitmentModel model)
        {
            await _employerCommitmentsOrchestrator.SubmitCommitment(model);

            return RedirectToAction("AcknowledgementExisting", new { hashedCommitmentId = model.HashedCommitmentId, message = model.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Submit")]
        public async Task<ActionResult> SubmitNewCommitmentEntry(SubmitCommitmentModel model)
        {
            var hashedCommitmentId = await _employerCommitmentsOrchestrator.CreateProviderAssignedCommitment(model);

            return RedirectToAction("AcknowledgementNew", new { hashedCommitmentId, providerName = model.ProviderName, legalEntityName = model.LegalEntityName, message = model.Message });
        }

        [HttpGet]
        [Route("Acknowledgement")]
        public ActionResult AcknowledgementNew(string hashedAccountId, string hashedCommitmentId, string providerName, string legalEntityName, string message)
        {
            return View("Acknowledgement", new AcknowledgementViewModel
            {
                HashedCommitmentId = hashedCommitmentId,
                ProviderName = providerName,
                LegalEntityName = legalEntityName,
                Message = message
            });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        public async Task<ActionResult> AcknowledgementExisting(string hashedAccountId, string hashedCommitmentId, string message)
        {
            var model = await _employerCommitmentsOrchestrator.Get(hashedAccountId, hashedCommitmentId);

            return View("Acknowledgement", new AcknowledgementViewModel
            {
                HashedCommitmentId = hashedCommitmentId,
                ProviderName = model.ProviderName,
                LegalEntityName = model.LegalEntityName,
                Message = message
            });
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

            ViewBag.ApprenticeshipProducts = model.ApprenticeshipProgrammes;

            return View(model.Apprenticeship);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedId}/Edit")]
        public async Task<ActionResult> EditApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return await RedisplayEditApprenticeshipView(apprenticeship);
                }

                await _employerCommitmentsOrchestrator.UpdateApprenticeship(apprenticeship);
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex);

                return await RedisplayEditApprenticeshipView(apprenticeship);
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
            ViewBag.ApprenticeshipProducts = model.ApprenticeshipProgrammes;

            return View("CreateApprenticeshipEntry", model.Apprenticeship);
        }

        private async Task<ActionResult> RedisplayEditApprenticeshipView(ApprenticeshipViewModel apprenticeship)
        {
            var model = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(apprenticeship.HashedAccountId, apprenticeship.HashedCommitmentId);
            model.Apprenticeship = apprenticeship;
            ViewBag.ApprenticeshipProducts = model.ApprenticeshipProgrammes;

            return View("EditApprenticeshipEntry", model.Apprenticeship);
        }

        private static string CreateReference()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }
    }
}
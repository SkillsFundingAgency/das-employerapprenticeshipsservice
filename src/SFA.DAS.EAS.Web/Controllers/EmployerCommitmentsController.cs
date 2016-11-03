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
        public ActionResult ConfirmProvider(string hashedAccountId, [System.Web.Http.FromUri]SelectProviderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = new SelectProviderViewModel
                {
                    LegalEntityCode = viewModel.LegalEntityCode,
                    CohortRef = viewModel.CohortRef
                };

                if (string.IsNullOrWhiteSpace(viewModel.ProviderId))
                {
                    return RedirectToAction("SearchProvider", model);
                }

                return View("SearchProvider", model);
            }

            return RedirectToAction("ChoosePath", new {hashedAccountId = hashedAccountId, legalEntityCode = viewModel.LegalEntityCode, providerId = viewModel.ProviderId, cohortRef = viewModel.CohortRef });
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
        public async Task<ActionResult> CreateCommitment(CreateCommitmentViewModel viewModel, string selectedRoute)
        {
            if (selectedRoute == "employer")
            {
                var hashedCommitmentId = await _employerCommitmentsOrchestrator.Create(viewModel, OwinWrapper.GetClaimValue(@"sub"));

                return RedirectToAction("Details", new { hashedCommitmentId = hashedCommitmentId });
            }

            return RedirectToAction("SubmitNewCommitment", new { hashedAccountId = viewModel.HashedAccountId, legalEntityCode = viewModel.LegalEntityCode, legalEntityName = viewModel.LegalEntityName, providerId = viewModel.ProviderId, providerName = viewModel.ProviderName, cohortRef = viewModel.CohortRef });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedCommitmentId)
        {
            var model = await _employerCommitmentsOrchestrator.Get(hashedAccountId, hashedCommitmentId);

            return View(model);
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Finished")]
        public ActionResult FinishedEditing(string hashedAccountId, string hashedCommitmentId)
        {
            var model = new SubmitCommitmentModel
            {
                HashedAccountId = hashedAccountId,
                HashedCommitmentId = hashedCommitmentId
            };

            return View(model);
        }

        [HttpPost]
        [Route("Finished")]
        public ActionResult FinishedCreating(string hashedAccountId, string legalEntityCode, string legalEntityName, string providerId, string providerName, string cohortRef, string saveOrSend)
        {
            if (saveOrSend == "save-no-send")
            {
                return RedirectToAction("Cohorts", new { hashedAccountId = hashedAccountId });
            }

            return RedirectToAction("SubmitNewCommitment", new { hashedAccountId = hashedAccountId, legalEntityCode = legalEntityCode, legalEntityName = legalEntityName, providerId = providerId, providerName = providerName, cohortRef = cohortRef, saveOrSend = saveOrSend });
        }

        [HttpPost]
        [Route("{hashedCommitmentId}/Finished")]
        public ActionResult FinishedEditingExistingChoice(string hashedAccountId, string hashedCommitmentId, string saveOrSend)
        {
            if (saveOrSend == "save-no-send")
            {
                return RedirectToAction("Cohorts", new {hashedAccountId = hashedAccountId});
            }

            return RedirectToAction("SubmitExistingCommitment", new { hashedAccountId = hashedAccountId, hashedCommitmentId = hashedCommitmentId, saveOrSend = saveOrSend});
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
        public ActionResult SubmitExistingCommitment(string hashedAccountId, string hashedCommitmentId, string saveOrSend)
        {
            var model = new SubmitCommitmentViewModel
            {
                HashedAccountId = hashedAccountId,
                HashedCommitmentId = hashedCommitmentId,
                SaveOrSend = saveOrSend
            };

            return View("SubmitCommitmentEntry", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Submit")]
        public async Task<ActionResult> SubmitExistingCommitmentEntry(SubmitCommitmentModel model)
        {
            await _employerCommitmentsOrchestrator.SubmitCommitment(model.HashedAccountId, model.HashedCommitmentId, model.LegalEntityCode, model.LegalEntityName, model.ProviderId, model.ProviderName, model.CohortRef, model.Message, model.SaveOrSend);

            return RedirectToAction("AcknowledgementExisting", new { hashedCommitmentId = model.HashedCommitmentId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Submit")]
        public async Task<ActionResult> SubmitNewCommitmentEntry(SubmitCommitmentModel model)
        {
            await _employerCommitmentsOrchestrator.SubmitCommitment(model.HashedAccountId, model.HashedCommitmentId, model.LegalEntityCode, model.LegalEntityName, model.ProviderId, model.ProviderName, model.CohortRef, model.Message, model.SaveOrSend);

            return RedirectToAction("AcknowledgementNew", new { hashedCommitmentId = model.HashedCommitmentId});
        }

        [HttpGet]
        [Route("Acknowledgement")]
        public ActionResult AcknowledgementNew(string hashedAccountId, string hashedCommitmentId)
        {
            return View("Acknowledgement", new AcknowledgementViewModel
            {
                HashedCommitmentId = hashedCommitmentId
            });
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        public ActionResult AcknowledgementExisting(string hashedAccountId, string hashedCommitmentId)
        {
            return View("Acknowledgement", new AcknowledgementViewModel
            {
                HashedCommitmentId = hashedCommitmentId
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
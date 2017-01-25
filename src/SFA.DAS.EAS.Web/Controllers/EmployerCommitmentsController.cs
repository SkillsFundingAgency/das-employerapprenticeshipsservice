using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Models;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.Models.Types;
using SFA.DAS.EAS.Web.Exceptions;

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
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            ViewBag.HashedAccountId = hashedAccountId;

            var response = await _employerCommitmentsOrchestrator.CheckAccountAuthorization(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpGet]
        [Route("Cohorts")]
        public async Task<ActionResult> Cohorts(string hashedAccountId)
        {
            var model = await _employerCommitmentsOrchestrator.GetAll(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpGet]
        [Route("Inform")]
        public async Task<ActionResult> Inform(string hashedAccountId)
        {
            var response = await _employerCommitmentsOrchestrator.GetInform(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpGet]
        [Route("Create/LegalEntity")]
        public async Task<ActionResult> SelectLegalEntity(string hashedAccountId, string cohortRef = "")
        {
            var response = await _employerCommitmentsOrchestrator.GetLegalEntities(hashedAccountId, cohortRef, OwinWrapper.GetClaimValue(@"sub"));

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create/LegalEntity")]
        public async Task<ActionResult> SetLegalEntity(string hashedAccountId, SelectLegalEntityViewModel selectedLegalEntity)
        {
            if (!ModelState.IsValid)
            {
                var response = await _employerCommitmentsOrchestrator.GetLegalEntities(hashedAccountId, selectedLegalEntity.CohortRef, OwinWrapper.GetClaimValue(@"sub"));

                return View("SelectLegalEntity", response);
            }

            return RedirectToAction("SearchProvider", selectedLegalEntity);
        }

        [HttpGet]
        [Route("Create/Provider")]
        public async Task<ActionResult> SearchProvider(string hashedAccountId, string legalEntityCode, string cohortRef)
        {
            var response = await _employerCommitmentsOrchestrator.GetProviderSearch(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), legalEntityCode, cohortRef);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create/Provider")]
        public async Task<ActionResult> SelectProvider(string hashedAccountId, [System.Web.Http.FromUri] SelectProviderViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var defaultViewModel = await _employerCommitmentsOrchestrator.GetProviderSearch(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.LegalEntityCode, viewModel.CohortRef);
                
                return View("SearchProvider", defaultViewModel);
            }

            var response = await _employerCommitmentsOrchestrator.GetProvider(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel);

            return View(response);
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
                    var response = await _employerCommitmentsOrchestrator.GetProvider(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel);

                    return View("SelectProvider", response);
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
            var model = await _employerCommitmentsOrchestrator.CreateSummary(hashedAccountId, legalEntityCode, providerId, cohortRef, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Create")]
        public async Task<ActionResult> CreateCommitment(CreateCommitmentViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var model = await _employerCommitmentsOrchestrator.CreateSummary(viewModel.HashedAccountId, viewModel.LegalEntityCode, viewModel.ProviderId.ToString(), viewModel.CohortRef,  OwinWrapper.GetClaimValue(@"sub"));

                return View("ChoosePath", model);
            }

            if (viewModel.SelectedRoute == "employer")
            {
                var response = await _employerCommitmentsOrchestrator.CreateEmployerAssignedCommitment(viewModel, OwinWrapper.GetClaimValue(@"sub"));

                return RedirectToAction("Details", new { hashedCommitmentId = response.Data });
            }

            return RedirectToAction("SubmitNewCommitment",
                new { hashedAccountId = viewModel.HashedAccountId, legalEntityCode = viewModel.LegalEntityCode, legalEntityName = viewModel.LegalEntityName, providerId = viewModel.ProviderId, providerName = viewModel.ProviderName, cohortRef = viewModel.CohortRef, saveStatus = SaveStatus.Save });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedCommitmentId)
        {
            var model = await _employerCommitmentsOrchestrator.GetCommitmentDetails(hashedAccountId, hashedCommitmentId, OwinWrapper.GetClaimValue(@"sub"));

            ViewBag.HashedAccountId = hashedAccountId;

            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Apprenticeships/Create")]
        public async Task<ActionResult> CreateApprenticeshipEntry(string hashedAccountId, string hashedCommitmentId)
        {
            var response = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId);

            return View(response);
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

                await _employerCommitmentsOrchestrator.CreateApprenticeship(apprenticeship, OwinWrapper.GetClaimValue(@"sub"));
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex);

                return await RedisplayCreateApprenticeshipView(apprenticeship);
            }

            return RedirectToAction("Details", new { hashedAccountId = apprenticeship.HashedAccountId, hashedCommitmentId = apprenticeship.HashedCommitmentId });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Apprenticeships/{hashedApprenticeshipId}/Edit")]
        public async Task<ActionResult> EditApprenticeship(string hashedAccountId, string hashedCommitmentId, string hashedApprenticeshipId)
        {
            var response = await _employerCommitmentsOrchestrator.GetApprenticeship(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, hashedApprenticeshipId);

            return View("EditApprenticeshipEntry", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Apprenticeships/{HashedApprenticeshipId}/Edit")]
        public async Task<ActionResult> EditApprenticeship(ApprenticeshipViewModel apprenticeship)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return await RedisplayEditApprenticeshipView(apprenticeship);
                }

                await _employerCommitmentsOrchestrator.UpdateApprenticeship(apprenticeship, OwinWrapper.GetClaimValue(@"sub"));
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex);

                return await RedisplayEditApprenticeshipView(apprenticeship);
            }

            return RedirectToAction("Details", new { hashedAccountId = apprenticeship.HashedAccountId, hashedCommitmentId = apprenticeship.HashedCommitmentId });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Finished")]
        public async Task<ActionResult> FinishedEditing(string hashedAccountId, string hashedCommitmentId)
        {
            var response = await _employerCommitmentsOrchestrator.GetFinishEditingViewModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId);

            return View(response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Finished")]
        public async Task<ActionResult> FinishedEditing(FinishEditingViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var response = await _employerCommitmentsOrchestrator.GetFinishEditingViewModel(viewModel.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.HashedCommitmentId);
                response.Data = viewModel;

                return View(response);
            }

            if(viewModel.SaveStatus.IsSend())
            {
                return RedirectToAction("SubmitExistingCommitment", 
                    new { viewModel.HashedAccountId, viewModel.HashedCommitmentId, viewModel.SaveStatus });
            }

            if (viewModel.SaveStatus.IsApproveWithoutSend())
            {
                await _employerCommitmentsOrchestrator.ApproveCommitment(viewModel.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), viewModel.HashedCommitmentId, viewModel.SaveStatus);
            }

            return RedirectToAction("Cohorts", new { hashedAccountId = viewModel.HashedAccountId });
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("Submit")]
        public async Task<ActionResult> SubmitNewCommitment(string hashedAccountId, string legalEntityCode, string legalEntityName, string providerId, string providerName, string cohortRef, SaveStatus saveStatus)
        {
            var response = await _employerCommitmentsOrchestrator.GetSubmitNewCommitmentModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), legalEntityCode, legalEntityName, providerId, providerName, cohortRef, saveStatus);

            return View("SubmitCommitmentEntry", response);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedCommitmentId}/Submit")]
        public async Task<ActionResult> SubmitExistingCommitment(string hashedAccountId, string hashedCommitmentId, SaveStatus saveStatus)
        {
            var response = await _employerCommitmentsOrchestrator.GetSubmitCommitmentModel(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, saveStatus);

            return View("SubmitCommitmentEntry", response);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Submit")]
        public async Task<ActionResult> SubmitExistingCommitmentEntry(SubmitCommitmentModel model)
        {
            await _employerCommitmentsOrchestrator.SubmitCommitment(model, OwinWrapper.GetClaimValue(@"sub"));

            return RedirectToAction("AcknowledgementExisting", new { hashedCommitmentId = model.HashedCommitmentId, message = model.Message });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("Submit")]
        public async Task<ActionResult> SubmitNewCommitmentEntry(SubmitCommitmentModel model)
        {
            var response = await _employerCommitmentsOrchestrator.CreateProviderAssignedCommitment(model, OwinWrapper.GetClaimValue(@"sub"));

            return RedirectToAction("AcknowledgementNew", new { response.Data, providerName = model.ProviderName, legalEntityName = model.LegalEntityName, message = model.Message });
        }

        [HttpGet]
        [Route("Acknowledgement")]
        public async Task<ActionResult> AcknowledgementNew(string hashedAccountId, string hashedCommitmentId, string providerName, string legalEntityName, string message)
        {
            var response = await _employerCommitmentsOrchestrator.GetAcknowledgementModelForNewCommitment(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, providerName, legalEntityName, message);

            // TODO: LWA - Is message in querystring a security issue?
            return View("Acknowledgement", response);
        }

        [HttpGet]
        [Route("{hashedCommitmentId}/Acknowledgement")]
        public async Task<ActionResult> AcknowledgementExisting(string hashedAccountId, string hashedCommitmentId, string message)
        {
            // TODO: LWA - Is message in querystring a security issue?
            var response = await _employerCommitmentsOrchestrator.GetAcknowledgementModelForExistingCommitment(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), hashedCommitmentId, message);

            return View("Acknowledgement", response);
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
            var response = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(apprenticeship.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship.HashedCommitmentId);
            response.Data.Apprenticeship = apprenticeship;

            return View("CreateApprenticeshipEntry", response);
        }

        private async Task<ActionResult> RedisplayEditApprenticeshipView(ApprenticeshipViewModel apprenticeship)
        {
            var response = await _employerCommitmentsOrchestrator.GetSkeletonApprenticeshipDetails(apprenticeship.HashedAccountId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship.HashedCommitmentId);
            response.Data.Apprenticeship = apprenticeship;

            return View("EditApprenticeshipEntry", response);
        }

        protected override void OnException(ExceptionContext filterContext)
        {
            if (filterContext.Exception is InvalidStateException)
            {
                filterContext.ExceptionHandled = true;
                filterContext.Result = RedirectToAction("Index", "Error");
            }
        }
    }
}
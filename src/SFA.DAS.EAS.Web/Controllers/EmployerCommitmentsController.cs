﻿using System;
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

            return RedirectToAction("SearchProvider", selectedLegalEntity);
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

            // The api returns a list but there should only ever be one per ukprn.
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

            if (selectedRoute == "employer")
            {
                return RedirectToAction("Details", new { hashedCommitmentId = hashedCommitmentId });
            }
            else
            {
                return RedirectToAction("SubmitCommitmentEntry", new { hashedCommitmentId = hashedCommitmentId });
            }
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
        [Route("{hashedCommitmentId}/FinishedEditing")]
        public ActionResult FinishedEditingChoice(string hashedAccountId, string hashedCommitmentId, string saveOrSend)
        {
            if (saveOrSend == "save-no-send")
            {
                return RedirectToAction("Cohorts", new {hashedAccountId = hashedAccountId});
            }

            return RedirectToAction("SubmitCommitmentEntry", new {hashedAccountId = hashedAccountId, hashedCommitmentId = hashedCommitmentId, saveOrSend = saveOrSend});
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
        public async Task<ActionResult> SubmitCommitmentEntry(string hashedAccountId, string hashedCommitmentId, string saveOrSend)
        {
            // TODO: LWA Implement 
            var commitment = await _employerCommitmentsOrchestrator.Get(hashedAccountId, hashedCommitmentId);
            
            var model = new SubmitCommitmentViewModel
            {
                SubmitCommitmentModel = new SubmitCommitmentModel
                {
                    HashedAccountId = hashedAccountId,
                    HashedCommitmentId = hashedCommitmentId
                },
                Commitment = commitment,
                SaveOrSend = saveOrSend
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedCommitmentId}/Submit")]
        public async Task<ActionResult> SubmitCommitment(SubmitCommitmentModel model, string saveOrSend)
        {
            // TODO: LWA Implement
            await _employerCommitmentsOrchestrator.SubmitCommitment(model.HashedAccountId, model.HashedCommitmentId, model.Message, saveOrSend);

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
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

using Newtonsoft.Json;

using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;

using WebGrease.Css.Extensions;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}/apprentices/manage")]
    public class EmployerManageApprenticesController : BaseController
    {
        private readonly EmployerManageApprenticeshipsOrchestrator _orchestrator;

        public EmployerManageApprenticesController(
            EmployerManageApprenticeshipsOrchestrator orchestrator, 
            IOwinWrapper owinWrapper,
            IFeatureToggle featureToggle, 
            IUserWhiteList userWhiteList)
                : base(owinWrapper, featureToggle, userWhiteList)
        {
            if (orchestrator == null)
                throw new ArgumentNullException(nameof(orchestrator));

            _orchestrator = orchestrator;
        }

        [HttpGet]
        [Route("all")]
        [OutputCache(CacheProfile = "NoCache")]
        public async Task<ActionResult> ListAll(string hashedAccountId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator
                .GetApprenticeships(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator
                .GetApprenticeship(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

            if (!string.IsNullOrEmpty(TempData["FlashMessage"]?.ToString()))
            {
                model.FlashMessage = JsonConvert.DeserializeObject<FlashMessageViewModel>(TempData["FlashMessage"].ToString());
            }

            return View(model);
        }

        [HttpGet]
        [OutputCache(CacheProfile = "NoCache")]
        [Route("{hashedApprenticeshipId}/edit", Name = "EditApprovedApprentice")]
        public async Task<ActionResult> Edit(string hashedAccountId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator
                .GetApprenticeshipForEdit(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpPost]
        [OutputCache(CacheProfile = "NoCache")]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/confirm")]
        public async Task<ActionResult> ConfirmChanges(ApprenticeshipViewModel apprenticeship)
        {
            if (!await IsUserRoleAuthorized(apprenticeship.HashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            AddErrorsToModelState(await _orchestrator.ValidateApprenticeship(apprenticeship));
            if (!ModelState.IsValid)
                return await RedisplayEditApprenticeshipView(apprenticeship, apprenticeship.HashedAccountId, apprenticeship.HashedApprenticeshipId);

            var model = await _orchestrator.GetConfirmChangesModel(apprenticeship.HashedAccountId, apprenticeship.HashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship);

            if (!AnyChanges(model.Data))
            {
                ModelState.AddModelError("NoChangesRequested", "No changes made");
                return await RedisplayEditApprenticeshipView(apprenticeship, apprenticeship.HashedAccountId, apprenticeship.HashedApprenticeshipId);
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/SubmitChanges")]
        public async Task<ActionResult> SubmitChanges(string hashedAccountId, string hashedApprenticeshipId, UpdateApprenticeshipViewModel apprenticeship, string originalApprenticeshipDecoded)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var originalApprenticeship = System.Web.Helpers.Json.Decode<Apprenticeship>(originalApprenticeshipDecoded);
            apprenticeship.OriginalApprenticeship = originalApprenticeship;

            if (!ModelState.IsValid)
            {
                var errorModel = new OrchestratorResponse<UpdateApprenticeshipViewModel> { Data = apprenticeship };
                return View("ConfirmChanges", errorModel);
            }

            if (apprenticeship.ChangesConfirmed != null && !apprenticeship.ChangesConfirmed.Value)
            {
                return RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });
            }
            
            _orchestrator.CreateApprenticeshipUpdate(apprenticeship, hashedAccountId, OwinWrapper.GetClaimValue(@"sub"));

            var flashmessage = new FlashMessageViewModel
            {
                Message = $"You suggested changes to the record for {originalApprenticeship.FirstName} {originalApprenticeship.LastName}. Your training provider needs to approve these changes.",
                Severity = FlashMessageSeverityLevel.Okay
            };
            TempData["FlashMessage"] = JsonConvert.SerializeObject(flashmessage);

            return RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/changes/view", Name = "ViewPendingChanges")]
        public ActionResult ViewChanges(string hashedAccountId, string hashedApprenticeshipId)
        {
            return View();
        }

        private async Task<ActionResult> RedisplayEditApprenticeshipView(ApprenticeshipViewModel apprenticeship, string hashedAccountId, string hashedApprenticeshipId)
        {
            var response = await _orchestrator.GetApprenticeshipForEdit(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
            response.Data.Apprenticeship = apprenticeship;

            return View("Edit", response);
        }

        private bool AnyChanges(UpdateApprenticeshipViewModel data)
        {
            return
                   !string.IsNullOrEmpty(data.FirstName)
                || !string.IsNullOrEmpty(data.LastName)
                || data.DateOfBirth != null
                || !string.IsNullOrEmpty(data.TrainingName)
                || data.StartDate != null
                || data.EndDate != null
                || data.Cost != null
                || !string.IsNullOrEmpty(data.EmployerRef);
        }

        private async Task<bool> IsUserRoleAuthorized(string hashedAccountId, params Role[] roles)
        {
            return await _orchestrator
                .AuthorizeRole(hashedAccountId, OwinWrapper.GetClaimValue(@"sub"), roles);
        }

        private void AddErrorsToModelState(Dictionary<string, string> dict)
        {
            dict.ForEach(error => ModelState.AddModelError(error.Key, error.Value));
        }
    }
}
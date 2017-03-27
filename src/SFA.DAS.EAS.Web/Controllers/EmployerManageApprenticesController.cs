using System;
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
        [Route("{hashedApprenticeshipId}/details")]
        public async Task<ActionResult> Details(string hashedAccountId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            var model = await _orchestrator
                .GetApprenticeship(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
            return View(model);
        }

        [HttpGet]
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
        [ValidateAntiForgeryToken]
        [Route("{hashedApprenticeshipId}/changes/confirm")]
        public async Task<ActionResult> ConfirmChanges(ApprenticeshipViewModel apprenticeship, string hashedAccountId, string hashedApprenticeshipId)
        {
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            // ToDo: Add overlapping errors to ModelState..
            if (!ModelState.IsValid)
                return await RedisplayEditApprenticeshipView(apprenticeship, hashedAccountId, hashedApprenticeshipId);

            var model = await _orchestrator.GetConfirmChangesModel(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship);

            if (!AnyChanges(model.Data))
            {
                ModelState.AddModelError("NoChangesRequested", "No changes made");
                return await RedisplayEditApprenticeshipView(apprenticeship, hashedAccountId, hashedApprenticeshipId);
            }

            return View(model);
        }

        [HttpPost]
        [Route("{hashedApprenticeshipId}/changes/SubmitChanges")]
        public async Task<ActionResult> SubmitChanges(string hashedAccountId, string hashedApprenticeshipId, UpdateApprenticeshipViewModel apprenticeship, string originalApprenticeshipDecoded)
        {
            //var d  = JsonConvert.DeserializeObject<Apprenticeship>()
            if (!await IsUserRoleAuthorized(hashedAccountId, Role.Owner, Role.Transactor))
                return View("AccessDenied");

            if (!ModelState.IsValid)
            {
                var d = System.Web.Helpers.Json.Decode<Apprenticeship>(originalApprenticeshipDecoded);
                apprenticeship.OriginalApprenticeship = d;

                var errorModel = new OrchestratorResponse<UpdateApprenticeshipViewModel> { Data = apprenticeship };
                return View("ConfirmChanges", errorModel);
            }

            if (apprenticeship.ChangesConfirmend != null && !apprenticeship.ChangesConfirmend.Value)
            {
                return RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });
            }
            // ToDo: Add overlapping errors to ModelState..
            //if (!ModelState.IsValid)
            //return await RedisplayEditApprenticeshipView(apprenticeship);

            // var model = await _orchestrator.GetConfirmChangesModel(hashedAccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"), apprenticeship);
            var flashmessage = new FlashMessageViewModel
            {
                Headline = "Changed saved",
                Message = "You successfully updated sent a change request",
                Severity = FlashMessageSeverityLevel.Success
            };
            TempData["FlashMessage"] = JsonConvert.SerializeObject(flashmessage);

            RedirectToAction("Details", new { hashedAccountId, hashedApprenticeshipId });

            return View();
        }

        // ToDo: Delete
        //[HttpGet]
        //[Route("{hashedApprenticeshipId}/changes/view", Name = "ViewPendingChanges")]
        //public ActionResult ViewChanges(string hashedAccountId, string hashedApprenticeshipId)
        //{
        //    return View();
        //}

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
    }
}
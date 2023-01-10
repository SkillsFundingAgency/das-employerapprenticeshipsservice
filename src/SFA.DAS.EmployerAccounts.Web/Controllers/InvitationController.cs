using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.EmployerAccounts.Web;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [RoutePrefix("invitations")]
    public class InvitationController : BaseController
    {
        private readonly InvitationOrchestrator _invitationOrchestrator;

        private readonly EmployerAccountsConfiguration _configuration;

        public InvitationController(InvitationOrchestrator invitationOrchestrator, IAuthenticationService owinWrapper, 
            IMultiVariantTestingService multiVariantTestingService,
            EmployerAccountsConfiguration configuration,
            ICookieStorageService<FlashMessageViewModel> flashMessage)
            : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            if (invitationOrchestrator == null)
                throw new ArgumentNullException(nameof(invitationOrchestrator));
            _invitationOrchestrator = invitationOrchestrator;
            _configuration = configuration;

        }

        [Route("invite")]
        public Microsoft.AspNetCore.Mvc.ActionResult Invite()
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName)))
            {
                return View();
            }

            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
        }

        [HttpGet]
        [DasAuthorize]
        [Route]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> All()
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName)))
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
            }

            var model = await _invitationOrchestrator.GetAllInvitationsForUser(OwinWrapper.GetClaimValue("sub"));

            return View(model);
        }

        [HttpGet]
        [DasAuthorize]
        [Route("view")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Details(string invitationId)
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName)))
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
            }

            var invitation = await _invitationOrchestrator.GetInvitation(invitationId);

            return View(invitation);
        }

        [HttpPost]
        [DasAuthorize]
        [ValidateAntiForgeryToken]
        [Route("accept")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Accept(long invitation, UserInvitationsViewModel model)
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName)))
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
            }

            var invitationItem = model.Invitations.SingleOrDefault(c => c.Id == invitation);

            if (invitationItem == null)
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
            }

            await _invitationOrchestrator.AcceptInvitation(invitationItem.Id, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            var flashMessage = new FlashMessageViewModel
            {
                Headline = "Invitation accepted",
                Message = $"You can now access the {invitationItem.AccountName} account",
                Severity = FlashMessageSeverityLevel.Success
            };
            AddFlashMessageToCookie(flashMessage);


            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
        }

        [HttpPost]
        [DasAuthorize]
        [ValidateAntiForgeryToken]
        [Route("create")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Create(InviteTeamMemberViewModel model)
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName)))
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
            }

            await _invitationOrchestrator.CreateInvitation(model, OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName));

            return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
        }

        [HttpGet]
        [Route("register-and-accept")]
        public Microsoft.AspNetCore.Mvc.ActionResult AcceptInvitationNewUser()
        {
            var schema = HttpContextHelper.Current.Request.Url.Scheme;
            var authority = HttpContextHelper.Current.Request.Url.Authority;
            var c = new Constants(_configuration.Identity);
            return new Microsoft.AspNetCore.Mvc.RedirectResult($"{c.RegisterLink()}{schema}://{authority}/invitations");
        }


        [HttpGet]
        [Route("accept")]
        public Microsoft.AspNetCore.Mvc.ActionResult AcceptInvitationExistingUser()
        {
            return RedirectToAction("All");
        }
    }
}
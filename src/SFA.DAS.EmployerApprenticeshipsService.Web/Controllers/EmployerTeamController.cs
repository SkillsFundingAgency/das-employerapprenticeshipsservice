using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Application;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    [Authorize]
    public class EmployerTeamController : Controller
    {

        private readonly IOwinWrapper _owinWrapper;
        private readonly EmployerTeamOrchestrator _employerTeamOrchestrator;

        public EmployerTeamController(IOwinWrapper owinWrapper, EmployerTeamOrchestrator employerTeamOrchestrator)
        {
            _owinWrapper = owinWrapper;
            _employerTeamOrchestrator = employerTeamOrchestrator;
        }

        [HttpGet]
        public async Task<ActionResult> Index(int accountId)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            var teamVieWModel = await _employerTeamOrchestrator.GetTeamMembers(accountId, userIdClaim.Value);
            return View(teamVieWModel);
        }

        [HttpGet]
        public ActionResult Invite(long accountId)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            var model = new InviteTeamMemberViewModel
            {
                AccountId = accountId
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Invite(InviteTeamMemberViewModel model)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            try
            {
                await _employerTeamOrchestrator.InviteTeamMember(model, userIdClaim.Value);
            }
            catch (InvalidRequestException ex)
            {
                AddErrorsToModelState(ex.ErrorMessages);
                return View(model);
            }
            catch (Exception ex)
            {
                AddExceptionToModelError(ex);
                return View(model);
            }

            return RedirectToAction("Index", new { accountId = model.AccountId });
        }

        private void AddErrorsToModelState(Dictionary<string, string> errors)
        {
            foreach (var error in errors)
            {
                ModelState.AddModelError(error.Key, error.Value);
            }
        }

        private void AddExceptionToModelError(Exception ex)
        {
            ModelState.AddModelError("", $"Unexpected exception: {ex.Message}");
        }
    }
}
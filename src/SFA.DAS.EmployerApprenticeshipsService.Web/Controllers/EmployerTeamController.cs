using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using Microsoft.ApplicationInsights.Web;
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
        public async Task<ActionResult> Index(int accountId)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            var teamVieWModel = await _employerTeamOrchestrator.GetTeamMembers(accountId, userIdClaim.Value);
            return View(teamVieWModel);
        }

        [HttpGet]
        public async Task<ActionResult> Review(int accountId, string email)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            var invitation = await _employerTeamOrchestrator.GetMember(accountId, email);

            return View(invitation);
        }

        [HttpPost]
        public async Task<ActionResult> Update(InvitationViewModel model)
        {
            var userIdClaim = ((ClaimsIdentity)System.Web.HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == @"sub");
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            //TODO: Update

            return RedirectToAction("Index", new { accountId = model.AccountId });
        }
    }
}
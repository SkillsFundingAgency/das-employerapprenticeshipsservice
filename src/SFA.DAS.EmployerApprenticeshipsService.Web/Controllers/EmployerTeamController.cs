using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
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
            var userIdClaim = _owinWrapper.GetPersistantUserIdClaimFromProvider();
            if (userIdClaim?.Value == null) return RedirectToAction("Index", "Home");

            var teamVieWModel = await _employerTeamOrchestrator.GetTeamMembers(accountId, userIdClaim.Value);
            return View(teamVieWModel);
        }
    }
}
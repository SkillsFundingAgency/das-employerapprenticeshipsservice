using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}/transfers")]
    public class TransfersController : Controller
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly TransferOrchestrator _orchestrator;

        public TransfersController(IOwinWrapper owinWrapper, TransferOrchestrator orchestrator)
        {
            _owinWrapper = owinWrapper;
            _orchestrator = orchestrator;
        }

        [Route]
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            var externalUserId = _owinWrapper.GetClaimValue(ControllerConstants.SubClaimKeyName);

            var response = await _orchestrator.GetTransferAllowance(hashedAccountId, externalUserId);

            return View(response);
        }
    }
}
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedaccountId}/apprentices/manage")]
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
        public async Task<ActionResult> ListAll(string hashedaccountId)
        {
            var model = await _orchestrator.GetApprenticeships(hashedaccountId, OwinWrapper.GetClaimValue(@"sub"));

            return View(model);
        }

        [HttpGet]
        [Route("{hashedApprenticeshipId}/details")]
        public async Task<ActionResult> Details(string hashedaccountId, string hashedApprenticeshipId)
        {
            var model = await _orchestrator.GetApprenticeship(hashedaccountId, hashedApprenticeshipId, OwinWrapper.GetClaimValue(@"sub"));
            return View(model);
        }
    }
}
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}")]
    public class EmployerTasksController : BaseController
    {
        private readonly IOwinWrapper _owinWrapper;
        private readonly EmployerTasksOrchestrator _employerTasksOrchestrator;

        public EmployerTasksController(IOwinWrapper owinWrapper, EmployerTasksOrchestrator employerTasksOrchestrator, IFeatureToggle featureToggle, IUserWhiteList userWhiteList) : base(owinWrapper, featureToggle, userWhiteList)
        {
            if (owinWrapper == null)
                throw new ArgumentNullException(nameof(owinWrapper));
            if (employerTasksOrchestrator == null)
                throw new ArgumentNullException(nameof(employerTasksOrchestrator));
            _owinWrapper = owinWrapper;
            _employerTasksOrchestrator = employerTasksOrchestrator;
        }

        [HttpGet]
        [Route("Tasks/List")]
        public async Task<ActionResult> Index(string hashedAccountId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");

            var response = await _employerTasksOrchestrator.GetTasks(hashedAccountId, userIdClaim);

            return View(response);
        }

        [HttpGet]
        [Route("Tasks/{hashedTaskId}")]
        public async Task<ActionResult> ViewTask(string hashedAccountId, string hashedTaskId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");

            var response = await _employerTasksOrchestrator.GetTask(hashedAccountId, hashedTaskId, userIdClaim);

            return View(response);
        }
    }
}

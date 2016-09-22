using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using SFA.DAS.EmployerApprenticeshipsService.Web.Orchestrators;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
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
        public async Task<ActionResult> Index(long accountId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");

            var response = await _employerTasksOrchestrator.GetTasks(accountId, userIdClaim);

            return View(response);
        }

        [HttpGet]
        public async Task<ActionResult> View(long accountId, long taskId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");

            var response = await _employerTasksOrchestrator.GetTask(accountId, taskId, userIdClaim);

            return View(response);
        }
    }
}

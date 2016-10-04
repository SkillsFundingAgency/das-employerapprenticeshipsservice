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
        public async Task<ActionResult> Index(string accountHashId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");

            var response = await _employerTasksOrchestrator.GetTasks(accountHashId, userIdClaim);

            return View(response);
        }

        [HttpGet]
        public async Task<ActionResult> View(string accountHashId, long taskId)
        {
            var userIdClaim = _owinWrapper.GetClaimValue(@"sub");

            var response = await _employerTasksOrchestrator.GetTask(accountHashId, taskId, userIdClaim);

            return View(response);
        }
    }
}

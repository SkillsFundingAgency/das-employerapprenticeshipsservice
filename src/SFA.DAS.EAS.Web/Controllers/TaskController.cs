using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("tasks/{hashedAccountId}")]
    public class TaskController : BaseController
    {
        private readonly TaskOrchestrator _orchestrator;
        private readonly ILog _logger;

        public TaskController(
            IOwinWrapper owinWrapper, 
            TaskOrchestrator taskOrchestrator,
            IFeatureToggle featureToggle, 
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage, 
            ILog logger) : base(owinWrapper, featureToggle, multiVariantTestingService, flashMessage)
        {
            _orchestrator = taskOrchestrator;
            _logger = logger;
        }

        [HttpGet]
        [Route("user/{hashedUserId}/task/{taskType}/dismiss", Name = "DismissTask")]
        public async Task<ActionResult> DismissTask(string hashedAccountId, string hashedUserId, string taskType)
        {
            _logger.Debug($"Task dismiss requested for account id {hashedAccountId}, user id {hashedUserId} and task {taskType}");
            var result = await _orchestrator.DismissMonthlyReminderTask(hashedAccountId, hashedUserId, taskType);
            
            if (result.Status != HttpStatusCode.OK)
            {
                //Curently we are not telling the user of the error and are instead just logging the issue
                //The error log will be done at a lower level
                _logger.Debug($"Task dismiss requested failed for account id {hashedAccountId}, user id {hashedUserId} and task {taskType}");
            }

            return RedirectToAction("Index", "EmployerTeam");
        }
    }
}
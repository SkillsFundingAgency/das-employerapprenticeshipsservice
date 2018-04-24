using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EmployerUsers.WebClientComponents;
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
            IAuthenticationService owinWrapper, 
            TaskOrchestrator taskOrchestrator,
            IAuthorizationService authorization, 
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage, 
            ILog logger) : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _orchestrator = taskOrchestrator;
            _logger = logger;
        }

        [HttpPost]
        [AuthoriseActiveUser]
        [Route("dismissTask", Name = "DismissTask")]
        public async Task<ActionResult> DismissTask(DismissTaskViewModel viewModel)
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue(ControllerConstants.UserExternalIdClaimKeyName)))
            {
                return RedirectToAction(ControllerConstants.IndexActionName, ControllerConstants.HomeControllerName);
            }

            var externalUserId = OwinWrapper.GetClaimValue("sub");

            _logger.Debug($"Task dismiss requested for account id '{viewModel.HashedAccountId}', user id '{externalUserId}' and task '{viewModel.TaskType}'");

            

            var result = await _orchestrator.DismissMonthlyReminderTask(viewModel.HashedAccountId, externalUserId, viewModel.TaskType);
            
            if (result.Status != HttpStatusCode.OK)
            {
                //Curently we are not telling the user of the error and are instead just logging the issue
                //The error log will be done at a lower level
                _logger.Debug($"Task dismiss requested failed for account id '{viewModel.HashedAccountId}', user id '{externalUserId}' and task '{viewModel.TaskType}'");
            }

            return RedirectToAction("Index", "EmployerTeam");
        }
    }
}
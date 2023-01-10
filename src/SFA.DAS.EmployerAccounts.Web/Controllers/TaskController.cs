using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Web.Helpers;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [DasAuthorize]
    [RoutePrefix("tasks/{hashedAccountId}")]
    public class TaskController : BaseController
    {
        private readonly TaskOrchestrator _orchestrator;
        private readonly ILog _logger;

        public TaskController(
            IAuthenticationService owinWrapper,
            TaskOrchestrator taskOrchestrator,
            IMultiVariantTestingService multiVariantTestingService,
            ICookieStorageService<FlashMessageViewModel> flashMessage,
            ILog logger) : base(owinWrapper, multiVariantTestingService, flashMessage)
        {
            _orchestrator = taskOrchestrator;
            _logger = logger;
        }

        [HttpPost]
        [DasAuthorize]
        [Route("dismissTask", Name = "DismissTask")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> DismissTask(DismissTaskViewModel viewModel)
        {
            if (string.IsNullOrEmpty(OwinWrapper.GetClaimValue(ControllerConstants.UserRefClaimKeyName)))
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
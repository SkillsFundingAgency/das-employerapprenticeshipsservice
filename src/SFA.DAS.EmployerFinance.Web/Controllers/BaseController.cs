using System;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Web.Helpers;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    public class BaseController : Controller
    {
        public IAuthenticationService OwinWrapper;

        public BaseController(IAuthenticationService owinWrapper)
        {
            OwinWrapper = owinWrapper;
        }

        protected override ViewResult View(string viewName, string masterName, object model)
        {
            var orchestratorResponse = model as OrchestratorResponse;

            if (orchestratorResponse == null)
            {
                return base.View(viewName, masterName, model);
            }

            var invalidRequestException = orchestratorResponse.Exception as InvalidRequestException;

            if (invalidRequestException != null)
            {
                foreach (var errorMessageItem in invalidRequestException.ErrorMessages)
                {
                    ModelState.AddModelError(errorMessageItem.Key, errorMessageItem.Value);
                }

                return ReturnViewResult(viewName, masterName, orchestratorResponse);
            }

            if (orchestratorResponse.Status == HttpStatusCode.BadRequest)
            {
                return ReturnViewResult(viewName, masterName, orchestratorResponse);
            }

            if (orchestratorResponse.Status == HttpStatusCode.NotFound)
            {
                return base.View(ControllerConstants.NotFoundViewName);
            }

            if (orchestratorResponse.Status == HttpStatusCode.OK)
            {
                return ReturnViewResult(viewName, masterName, orchestratorResponse);
            }

            if (orchestratorResponse.Status == HttpStatusCode.Unauthorized)
            {
                var accountId = Request.Params[ControllerConstants.AccountHashedIdRouteKeyName];

                if (accountId != null)
                {
                    ViewBag.AccountId = accountId;
                }

                return base.View(ControllerConstants.AccessDeniedViewName, masterName, orchestratorResponse);
            }

            if (orchestratorResponse.Exception != null)
            {
                throw orchestratorResponse.Exception;
            }

            throw new Exception($"Orchestrator response of type '{model.GetType()}' could not be handled.");
        }
        private ViewResult ReturnViewResult(string viewName, string masterName, OrchestratorResponse orchestratorResponse)
        {
            return base.View(viewName, masterName, orchestratorResponse);
        }
    }
}
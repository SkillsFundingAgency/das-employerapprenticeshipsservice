using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using SFA.DAS.EmployerApprenticeshipsService.Web.Models;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    public class BaseController : Controller
    {
        protected override ViewResult View(string viewName, string masterName, object model)
        {
            var orchestratorResponse = model as OrchestratorResponse;
            if (orchestratorResponse == null) return base.View(viewName, masterName, model);

            var flashMessage = GetHomePageSucessMessage();
            if (flashMessage != null)
            {
                orchestratorResponse.FlashMessage = flashMessage;
            }
            if (orchestratorResponse.Status == HttpStatusCode.OK)
                return base.View(viewName, masterName, orchestratorResponse);

            if (orchestratorResponse.Status == HttpStatusCode.Unauthorized)
                return base.View(@"AccessDenied", masterName, orchestratorResponse);

            return base.View(@"GenericError", masterName, orchestratorResponse);

        }

        protected FlashMessageViewModel GetHomePageSucessMessage()
        {
            if (TempData.ContainsKey("successHeader") || TempData.ContainsKey("successMessage"))
            {
                var successMessageViewModel = new FlashMessageViewModel();
                object message;
                successMessageViewModel.Severity = FlashMessageSeverityLevel.Success;
                if (TempData.TryGetValue("successHeader", out message))
                {
                    successMessageViewModel.Headline = message.ToString();
                }
                if (TempData.TryGetValue("successCompany", out message))
                {
                    successMessageViewModel.Message = message.ToString();
                }
                if (TempData.TryGetValue("successMessage", out message))
                {
                    successMessageViewModel.SubMessage = message.ToString();
                }
                return successMessageViewModel;
            }
            return null;
        }


    }
}
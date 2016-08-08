using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Controllers
{
    public class BaseController : Controller
    {
        protected override ViewResult View(string viewName, string masterName, object model)
        {
            var orchestratorResponse = model as OrchestratorResponse;
            if (orchestratorResponse == null) return base.View(viewName, masterName, model);

            if (orchestratorResponse.Status == HttpStatusCode.OK)
                return base.View(viewName, masterName, orchestratorResponse);

            if (orchestratorResponse.Status == HttpStatusCode.Unauthorized)
                return base.View(@"AccessDenied", masterName, orchestratorResponse);

            return base.View(@"GenericError", masterName, orchestratorResponse);

        }


    }
}
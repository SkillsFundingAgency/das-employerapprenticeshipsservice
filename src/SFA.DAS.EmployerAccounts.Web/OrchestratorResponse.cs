﻿using SFA.DAS.EmployerAccounts.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Web
{
    public class OrchestratorResponse
    {
        public OrchestratorResponse()
        {
            this.Status = HttpStatusCode.OK;
        }
        public HttpStatusCode Status { get; set; }
        public Exception Exception { get; set; }
        public FlashMessageViewModel FlashMessage { get; set; }

        public string RedirectUrl { get; set; }
        public string RedirectButtonMessage { get; set; }
    }

    public class OrchestratorResponse<T> : OrchestratorResponse
    {
        public T Data { get; set; }
    }
}

using System;
using System.Net;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web
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
using System;
using System.Net;
using System.Runtime.InteropServices;

namespace SFA.DAS.EmployerApprenticeshipsService.Web
{
    public class OrchestratorResponse
    {
        public HttpStatusCode Status { get; set; }
        public Exception Exception { get; set; }
    }

    public class OrchestratorResponse<T> : OrchestratorResponse
    {
        public T Data { get; set; }
    }

}
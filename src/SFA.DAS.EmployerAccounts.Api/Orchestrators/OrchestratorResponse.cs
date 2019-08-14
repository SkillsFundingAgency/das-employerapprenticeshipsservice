using System;
using System.Net;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
{
    public class OrchestratorResponse
    {
        public OrchestratorResponse()
        {
            Status = HttpStatusCode.OK;
        }

        public HttpStatusCode Status { get; set; }
        public Exception Exception { get; set; }
    }

    public class OrchestratorResponse<T> : OrchestratorResponse
    {
        public T Data { get; set; }
    }
}
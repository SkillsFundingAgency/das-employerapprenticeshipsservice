using System;
using System.Runtime.InteropServices;

namespace SFA.DAS.EmployerApprenticeshipsService.Web
{
    public class OrchestratorResponse
    {
        public OrchestratorResponseStatus Status { get; set; }
        public Exception Exception { get; set; }
    }

    public class OrchestratorResponse<T> : OrchestratorResponse
    {
        public T Data { get; set; }
    }

    public enum OrchestratorResponseStatus
    {
        Unknown = 0,
        Success,
        NotAMember,
        NotAnOwner
    }
}
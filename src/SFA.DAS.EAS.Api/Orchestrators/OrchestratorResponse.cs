using System.Net;

namespace SFA.DAS.EAS.Account.Api.Orchestrators;

public class OrchestratorResponse
{
    protected OrchestratorResponse()
    {
        Status = HttpStatusCode.OK;
    }

    public HttpStatusCode Status { get; set; }
}

public class OrchestratorResponse<T> : OrchestratorResponse
{
    public T Data { get; set; }
}
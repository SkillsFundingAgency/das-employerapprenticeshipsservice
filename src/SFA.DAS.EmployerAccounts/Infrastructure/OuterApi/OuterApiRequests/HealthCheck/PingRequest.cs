using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.OuterApiRequests.HealthCheck;

public class PingRequest : IGetApiRequest
{
    public string GetUrl => "ping";

    public PingRequest()
    {
    }
}
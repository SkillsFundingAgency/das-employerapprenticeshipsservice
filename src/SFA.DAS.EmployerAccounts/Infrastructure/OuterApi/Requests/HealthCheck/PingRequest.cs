using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.HealthCheck;

public class PingRequest : IGetApiRequest
{
    public string GetUrl => "ping";

    public PingRequest()
    {
    }
}
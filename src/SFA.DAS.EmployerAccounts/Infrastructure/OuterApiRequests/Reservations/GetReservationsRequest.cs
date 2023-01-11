using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.Infrastructure.OuterApiRequests.Reservations;

public class GetReservationsRequest : IGetApiRequest
{
    public long AccountId;
    public string GetUrl => $"reservation/{AccountId}";
    public GetReservationsRequest(long accountId)
    {
        AccountId = accountId;
    }
}
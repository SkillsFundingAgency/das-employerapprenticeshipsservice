using SFA.DAS.EmployerAccounts.Interfaces.OuterApi;

namespace SFA.DAS.EmployerAccounts.Infrastructure.OuterApi.Requests.Reservations;

public class GetReservationsRequest : IGetApiRequest
{
    public long AccountId { get; }
    public string GetUrl => $"reservation/{AccountId}";

    public GetReservationsRequest(long accountId)
    {
        AccountId = accountId;
    }
}
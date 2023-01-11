namespace SFA.DAS.EmployerAccounts.Queries.GetReservations;

public class GetReservationsRequest : IAsyncRequest<GetReservationsResponse>
{
    public string HashedAccountId { get; set; }
    public string ExternalUserId { get; set; }
}
namespace SFA.DAS.EmployerAccounts.Queries.GetReservations;

public class GetReservationsRequest : IRequest<GetReservationsResponse>
{
    public long AccountId { get; set; }
    public string ExternalUserId { get; set; }
}
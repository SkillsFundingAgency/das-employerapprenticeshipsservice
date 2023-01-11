namespace SFA.DAS.EmployerAccounts.Queries.GetProviderInvitation;

public class GetProviderInvitationQuery : IAsyncRequest<GetProviderInvitationResponse>
{
    public Guid CorrelationId { get; set; }
}
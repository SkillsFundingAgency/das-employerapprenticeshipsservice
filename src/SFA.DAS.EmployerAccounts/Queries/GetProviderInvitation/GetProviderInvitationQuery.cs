namespace SFA.DAS.EmployerAccounts.Queries.GetProviderInvitation;

public class GetProviderInvitationQuery : IRequest<GetProviderInvitationResponse>
{
    public Guid CorrelationId { get; set; }
}
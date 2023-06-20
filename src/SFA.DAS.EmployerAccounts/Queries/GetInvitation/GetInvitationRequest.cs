namespace SFA.DAS.EmployerAccounts.Queries.GetInvitation;

public class GetInvitationRequest : IRequest<GetInvitationResponse>
{
    public long Id { get; set; }
}
namespace SFA.DAS.EmployerAccounts.Queries.GetInvitation;

public class GetInvitationRequest : IRequest<GetInvitationResponse>
{
    public string Id { get; set; }
}
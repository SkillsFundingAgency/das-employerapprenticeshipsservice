namespace SFA.DAS.EmployerAccounts.Queries.GetInvitation;

public class GetInvitationRequest : IAsyncRequest<GetInvitationResponse>
{
    public string Id { get; set; }
}
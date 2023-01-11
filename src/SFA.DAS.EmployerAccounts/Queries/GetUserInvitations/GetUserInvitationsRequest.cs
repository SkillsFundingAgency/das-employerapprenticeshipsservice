namespace SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;

public class GetUserInvitationsRequest : IAsyncRequest<GetUserInvitationsResponse>
{
    public string UserId { get; set; }
}
namespace SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;

public class GetNumberOfUserInvitationsQuery : IAsyncRequest<GetNumberOfUserInvitationsResponse>
{
    public string UserId { get; set; }
}
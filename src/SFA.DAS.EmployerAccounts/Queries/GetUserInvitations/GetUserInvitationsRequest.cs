namespace SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;

public class GetUserInvitationsRequest : IRequest<GetUserInvitationsResponse>
{
    public string UserId { get; set; }
}
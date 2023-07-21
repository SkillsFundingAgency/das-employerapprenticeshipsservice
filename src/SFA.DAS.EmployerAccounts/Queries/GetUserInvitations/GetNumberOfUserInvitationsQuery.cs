namespace SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;

public class GetNumberOfUserInvitationsQuery : IRequest<GetNumberOfUserInvitationsResponse>
{
    public string UserId { get; set; }
}
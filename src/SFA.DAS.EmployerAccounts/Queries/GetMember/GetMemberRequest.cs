namespace SFA.DAS.EmployerAccounts.Queries.GetMember;

public class GetMemberRequest : IRequest<GetMemberResponse>
{
    public long AccountId { get; set; }
    public string Email { get; set; }

    /// <summary>
    ///     Set this true if you only want to return the member if they are active.
    ///     Default is to return the member even if they are tentative (i.e. have 
    ///     - not accepted the invitation).
    /// </summary>
    public bool OnlyIfMemberIsActive { get; set; }
}
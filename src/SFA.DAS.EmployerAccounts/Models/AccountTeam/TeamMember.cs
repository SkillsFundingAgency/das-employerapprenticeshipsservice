namespace SFA.DAS.EmployerAccounts.Models.AccountTeam;

public class TeamMember : IAccountIdentifier
{
    public bool IsUser { get; set; }
    public long Id { get; set; }
    public long AccountId { get; set; }
    public string HashedAccountId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Name { get; set; }
    public Guid UserRef { get; set; }
    public Role Role { get; set; }
    public InvitationStatus Status { get; set; }
    public DateTime ExpiryDate { get; set; }
    public string HashedInvitationId { get; set; }
    public bool CanReceiveNotifications { get; set; }
}
namespace SFA.DAS.EmployerAccounts.Models.Account;

public class Account
{
    public virtual long Id { get; set; }
    public virtual ICollection<AccountLegalEntity> AccountLegalEntities { get; set; } = new List<AccountLegalEntity>();
    public virtual DateTime CreatedDate { get; set; }
    public virtual string HashedId { get; set; }
    public virtual DateTime? ModifiedDate { get; set; }
    public virtual string Name { get; set; }
    public virtual bool NameConfirmed { get; set; }
    public virtual string PublicHashedId { get; set; }
    public virtual Role Role { get; set; }
    public string RoleName => (Role).ToString();
    public virtual byte ApprenticeshipEmployerType { get; set; }
    public virtual ICollection<Membership> Memberships { get; set; } = new List<Membership>();
    public virtual ICollection<AccountHistory> AccountHistory { get; set; } = new List<AccountHistory>();
}
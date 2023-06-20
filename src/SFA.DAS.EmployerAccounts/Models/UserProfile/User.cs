namespace SFA.DAS.EmployerAccounts.Models.UserProfile;

public class User
{
    public virtual long Id { get; set; }

    public virtual Guid Ref
    {
        get => _ref ?? Guid.Parse(_userRef);
        set => _ref = value;
    }

    [Obsolete("Please use 'Ref' instead.")]
    public string UserRef
    {
        get => _userRef ?? _ref.Value.ToString();
        set => _userRef = value;
    }

    public virtual string Email { get; set; }
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
    public virtual string CorrelationId { get; set; }
    public virtual DateTime? TermAndConditionsAcceptedOn { get; set; }
    public string FullName => $"{FirstName} {LastName}";
    public virtual ICollection<Membership> Memberships { get; protected set; } = new List<Membership>();
    public virtual ICollection<UserAccountSetting> UserAccountSettings { get; protected set; } = new List<UserAccountSetting>();
        
    private Guid? _ref;
    private string _userRef;
}
namespace SFA.DAS.EmployerAccounts.Models.Account;

public class AccountLegalEntity
{
    public virtual long Id { get; set; }
    public virtual Account Account { get; set; }
    public virtual long AccountId { get; set; }
    public virtual string Address { get; set; }
    public virtual ICollection<EmployerAgreement> Agreements { get; set; } = new List<EmployerAgreement>();
    public virtual DateTime Created { get; set; }
    public virtual LegalEntity LegalEntity { get; set; }
    public virtual long LegalEntityId { get; set; }
    public virtual DateTime? Modified { get; set; }
    public virtual string Name { get; set; }
    public virtual EmployerAgreement PendingAgreement { get; set; }
    public virtual long? PendingAgreementId { get; set; }
        
    /// <summary>
    ///     The version number of the latest agreement template that is pending. If this agreement is signed then this 
    ///     property will revert to null.
    /// </summary>
    public virtual int? PendingAgreementVersion { get; set; }

    public virtual string PublicHashedId { get; set; }
    public virtual EmployerAgreement SignedAgreement { get; set; }
    public virtual long? SignedAgreementId { get; set; }

    /// <summary>
    ///     The version number of the last agreement template signed by the legal entity for the account.
    /// </summary>
    public virtual int? SignedAgreementVersion { get; set; }

    /// <summary>
    ///     If this has a value then the ALE is logically deleted. Note that logically deleted ALE may still
    ///     have the signed and pending fields set so do not rely on these being null.
    /// </summary>
    public virtual DateTime? Deleted { get; set; }
}
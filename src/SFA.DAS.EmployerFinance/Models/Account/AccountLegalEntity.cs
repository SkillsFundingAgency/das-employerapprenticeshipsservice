namespace SFA.DAS.EmployerFinance.Models.Account
{
    public class AccountLegalEntity
    {
        public virtual long Id { get; set; }
        public virtual Account Account { get; set; }
        public virtual long AccountId { get; set; }
        public virtual long? PendingAgreementId { get; set; }
        public virtual long? SignedAgreementId { get; set; }

        /// <summary>
        ///     The version number of the last agreement template signed by the legal entity for the account.
        /// </summary>
        public virtual int? SignedAgreementVersion { get; set; }
    }
}

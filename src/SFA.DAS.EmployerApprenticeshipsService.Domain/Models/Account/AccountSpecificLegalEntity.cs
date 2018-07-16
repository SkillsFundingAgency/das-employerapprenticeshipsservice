using System;

namespace SFA.DAS.EAS.Domain.Models.Account
{
    /// <summary>
    ///     Represents the combination of a <see cref="LegalEntity"/> and the instance of a <see cref="AccountLegalEntity"/>
    ///     corresponding to the current account.
    /// </summary>
    public class AccountSpecificLegalEntity
    {
        #region Legal Entity properties common to all accounts
        public long Id { get; set; }
        public string HashedId { get; set; }
        public string Code { get; set; }
        public DateTime? DateOfIncorporation { get; set; }
        public byte? PublicSectorDataSource { get; set; }
        public string Sector { get; set; }
        public byte Source { get; set; }
        public string Status { get; set; }
        #endregion

        #region Account legal entity properties specific to account
        public string Name { get; set; }
        public string Address { get; set; }
        public int? SignedAgreementVersion { get; set; }
        public long? SignedAgreementId { get; set; }
        public int? PendingAgreementVersion { get; set; }
        public long? PendingAgreementId { get; set; }
        public long AccountLegalEntityId { get; set; }
        public string AccountLegalEntityPublicHashedId { get; set; }
        #endregion
    }
}
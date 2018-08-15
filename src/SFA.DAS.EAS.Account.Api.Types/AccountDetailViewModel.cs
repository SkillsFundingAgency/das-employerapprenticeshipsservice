using System;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class AccountDetailViewModel : IAccountResource
    {
        public long AccountId { get; set; }
        public string HashedAccountId { get; set; }
        public string PublicHashedAccountId { get; set; }
        public string DasAccountName { get; set; }
        public DateTime DateRegistered { get; set; }
        public string OwnerEmail { get; set; }
        public ResourceList LegalEntities { get; set; }
        public ResourceList PayeSchemes { get; set; }
        public decimal Balance { get; set; }
        public decimal TransferAllowance { get; set; }
        [Obsolete]
        public string DasAccountId => HashedAccountId;
    }
}

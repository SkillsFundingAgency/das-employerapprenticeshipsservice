using System;

namespace SFA.DAS.EAS.Finance.Api.Types
{
    public class AccountDetailViewModel
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

        [Obsolete("This property is now being replaced by RemainingTransferAllowance")]
        public decimal TransferAllowance
        {
            get => RemainingTransferAllowance;
            set => RemainingTransferAllowance = value;
        }

        public decimal RemainingTransferAllowance { get; set; }

        public decimal StartingTransferAllowance { get; set; }

        [Obsolete]
        public string DasAccountId => HashedAccountId;

        public AccountAgreementType AccountAgreementType { get; set; }

        public string ApprenticeshipEmployerType { get; set; }
        public bool IsAllowedPaymentOnService { get; set; }
    }
}

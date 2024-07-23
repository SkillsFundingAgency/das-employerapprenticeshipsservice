using System;
using SFA.DAS.Common.Domain.Types;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class AccountWithBalanceViewModel
    {
        public string AccountName { get; set; }

        public string HashedAccountId { get; set; }

        public string PublicAccountHashId { get; set; }

        public long AccountId { get; set; }

        public decimal Balance { get; set; }

        [Obsolete("This property is now being replaced by RemainingTransferAllowance")]
        public decimal TransferAllowance
        {
            get => RemainingTransferAllowance;
            set => RemainingTransferAllowance = value;
        }

        public decimal RemainingTransferAllowance { get; set; }

        public decimal StartingTransferAllowance { get; set; }

        public string Href { get; set; }

        [Obsolete("Use AllowedOnService instead")]
        public bool IsLevyPayer { get; set; }
        public bool IsAllowedPaymentOnService { get; set; }
        public AccountAgreementType AccountAgreementType { get; set; }
        public ApprenticeshipEmployerType ApprenticeshipEmployerType { get; set; }

    }
}

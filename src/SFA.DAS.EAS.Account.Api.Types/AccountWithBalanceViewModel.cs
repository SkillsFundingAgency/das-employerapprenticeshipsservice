using System;

namespace SFA.DAS.EAS.Account.Api.Types
{
    public class AccountWithBalanceViewModel : IAccountResource
    {
        public string AccountName { get; set; }

        public string AccountHashId { get; set; }

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
        public bool IsLevyPayer { get; set; }

    }
}

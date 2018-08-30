namespace SFA.DAS.EmployerAccounts.Models.Transfers
{
    public class TransferAllowance
    {
        private decimal? _remainingAllowance;

        public decimal? StartingTransferAllowance { get; set; }

        public decimal? RemainingTransferAllowance
        {
            get => 1000m;
            set => _remainingAllowance = value;
        }
    }
}

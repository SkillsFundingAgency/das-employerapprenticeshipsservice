namespace SFA.DAS.EmployerFinance.Api.Types
{
    public class TransferAllowance
    {
        private decimal? _remainingAllowance;

        public decimal? StartingTransferAllowance { get; set; }

        public decimal? RemainingTransferAllowance
        {
            get => _remainingAllowance < 0 ? 0 : _remainingAllowance;
            set => _remainingAllowance = value;
        }
    }
}

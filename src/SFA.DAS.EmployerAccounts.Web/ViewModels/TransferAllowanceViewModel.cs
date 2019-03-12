namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class TransferAllowanceViewModel
    {

        public decimal RemainingTransferAllowance { get; set; }
        public decimal StartingTransferAllowance { get; set; }
        public decimal PercentLevyTransferAllowance { get => _PercentLevyTransferAllowance * 100; set => _PercentLevyTransferAllowance = value; }
        private decimal _PercentLevyTransferAllowance;
    }
}
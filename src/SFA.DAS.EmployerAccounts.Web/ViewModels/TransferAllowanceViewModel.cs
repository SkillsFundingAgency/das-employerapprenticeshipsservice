namespace SFA.DAS.EmployerAccounts.Web.ViewModels
{
    public class TransferAllowanceViewModel
    {

        public decimal RemainingTransferAllowance { get; set; }
        public decimal StartingTransferAllowance { get; set; }
        public decimal TransferAllowancePercentage { get => _TransferAllowancePercentage * 100; set => _TransferAllowancePercentage = value; }
        private decimal _TransferAllowancePercentage;
    }
}
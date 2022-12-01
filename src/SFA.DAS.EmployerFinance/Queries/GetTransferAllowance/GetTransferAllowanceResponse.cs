using SFA.DAS.EmployerFinance.Models.Transfers;

namespace SFA.DAS.EmployerFinance.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceResponse
    {
        public TransferAllowance TransferAllowance { get; set; }
        public decimal TransferAllowancePercentage { get; set; }
    }
}
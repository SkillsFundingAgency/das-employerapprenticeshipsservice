using SFA.DAS.EmployerAccounts.Models.Transfers;

namespace SFA.DAS.EmployerAccounts.Queries.GetTransferAllowance
{
    public class GetTransferAllowanceResponse
    {
        public TransferAllowance TransferAllowance { get; set; }
        public decimal TransferAllowancePercentage { get; set; }
    }
}
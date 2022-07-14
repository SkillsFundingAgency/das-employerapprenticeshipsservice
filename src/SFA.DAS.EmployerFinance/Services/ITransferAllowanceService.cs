using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Transfers;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface ITransferAllowanceService
    {
        Task<TransferAllowance> GetTransferAllowance(long accountId);
    }
}
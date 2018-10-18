using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.Transfers;

namespace SFA.DAS.EmployerAccounts.Services
{
    public interface ITransferAllowanceService
    {
        Task<TransferAllowance> GetTransferAllowance(long accountId);
    }
}
using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.Transfers;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ITransferAllowanceService
    {
        Task<TransferAllowance> GetTransferAllowance(long accountId);
    }
}
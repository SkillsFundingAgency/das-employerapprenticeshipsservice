using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface ITransferAllowanceService
    {
        Task<decimal> GetTransferAllowance(long accountId);
    }
}
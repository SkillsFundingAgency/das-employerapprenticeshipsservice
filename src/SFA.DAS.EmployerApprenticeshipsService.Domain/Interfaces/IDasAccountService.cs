using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.PAYE;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IDasAccountService
    {
        Task<PayeSchemes> GetAccountSchemes(long accountId);
    }
}
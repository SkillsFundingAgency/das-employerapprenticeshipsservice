using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IDasAccountService
    {
        Task<Schemes> GetAccountSchemes(long accountId);
    }
}
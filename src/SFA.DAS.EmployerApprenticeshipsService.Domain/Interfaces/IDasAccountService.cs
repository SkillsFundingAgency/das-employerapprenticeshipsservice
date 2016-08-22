using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces
{
    public interface IDasAccountService
    {
        Task<Schemes> GetAccountSchemes(long accountId);
    }
}
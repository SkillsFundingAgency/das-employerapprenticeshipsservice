using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Paye;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IDasAccountService
    {
        Task<PayeSchemes> GetAccountSchemes(long accountId);
        Task UpdatePayeScheme(string empRef);
    }
}
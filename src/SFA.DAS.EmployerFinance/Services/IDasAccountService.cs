using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IDasAccountService
    {
        Task UpdatePayeScheme(string empRef);
    }
}
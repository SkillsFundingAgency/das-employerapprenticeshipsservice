using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Interfaces
{
    public interface IEmployerFinanceApiClient
    {
        Task HealthCheck();
    }
}
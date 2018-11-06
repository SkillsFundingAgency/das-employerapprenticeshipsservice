using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IEmployerAccountsApiClient
    {
        Task HealthCheck();
    }
}
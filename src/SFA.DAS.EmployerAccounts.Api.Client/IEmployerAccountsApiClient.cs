using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Api.Client
{
    public interface IEmployerAccountsApiClient
    {
        Task HealthCheck();
    }
}
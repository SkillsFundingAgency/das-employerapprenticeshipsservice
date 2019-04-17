using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IProviderService
    {
        Task<Models.ApprenticeshipProvider.Provider> Get(long ukPrn);
    }
}

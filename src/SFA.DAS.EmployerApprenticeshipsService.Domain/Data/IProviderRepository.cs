using System.Threading.Tasks;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IProviderRepository
    {
        Task<Providers> GetAllProviders();
    }
}
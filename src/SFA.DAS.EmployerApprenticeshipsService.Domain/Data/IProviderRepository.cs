using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IProviderRepository
    {
        Task<Providers> GetAllProviders();
    }
}
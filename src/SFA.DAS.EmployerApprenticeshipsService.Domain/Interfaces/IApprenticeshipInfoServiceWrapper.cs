using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IApprenticeshipInfoServiceWrapper
    {
        Task<StandardsView> GetStandardsAsync(bool refreshCache = false);
        Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false);
        ProvidersView GetProvider(int ukPrn);
    }
}
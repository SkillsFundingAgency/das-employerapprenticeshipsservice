using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IApprenticeshipInfoServiceWrapper
    {
        Task<StandardsView> GetStandardsAsync(bool refreshCache = false);
        Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false);
        ProvidersView GetProvider(long ukPrn);
    }
}
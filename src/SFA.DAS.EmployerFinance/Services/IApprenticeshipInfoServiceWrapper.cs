using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.ApprenticeshipCourse;

namespace SFA.DAS.EmployerFinance.Services
{
    public interface IApprenticeshipInfoServiceWrapper
    {
        Task<StandardsView> GetStandardsAsync(bool refreshCache = false);
        Task<FrameworksView> GetFrameworksAsync(bool refreshCache = false);      
    }
}
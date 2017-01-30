using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IStandardsRepository
    {
        Task<Standard[]> GetAllAsync();
        Task<Standard> GetByCodeAsync(string code);
    }
}
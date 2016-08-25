using System.Threading.Tasks;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Models.Levy;

namespace SFA.DAS.EmployerApprenticeshipsService.Domain.Data
{
    public interface IEnglishFractionRepository
    {
        Task<DasEnglishFraction> GetLatest(string empRef);

        Task Save(DasEnglishFraction fraction);
    }
}

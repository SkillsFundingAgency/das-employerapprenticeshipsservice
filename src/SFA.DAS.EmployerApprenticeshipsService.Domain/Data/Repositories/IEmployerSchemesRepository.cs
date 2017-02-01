using System.Threading.Tasks;
using SFA.DAS.EAS.Domain.Models.PAYE;

namespace SFA.DAS.EAS.Domain.Data.Repositories
{
    public interface IEmployerSchemesRepository
    {
        Task<PayeSchemes> GetSchemesByEmployerId(long employerId);
        Task<PayeScheme> GetSchemeByRef(string empref);
    }
}
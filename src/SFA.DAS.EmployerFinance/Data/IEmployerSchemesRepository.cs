using System.Threading.Tasks;
using SFA.DAS.EmployerFinance.Models.Paye;

namespace SFA.DAS.EmployerFinance.Data
{
    public interface IEmployerSchemesRepository
    {
        Task<PayeSchemes> GetSchemesByEmployerId(long employerId);
    }
}
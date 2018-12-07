using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Models.PAYE;

namespace SFA.DAS.EmployerAccounts.Data
{
    public interface IEmployerSchemesRepository
    {
        //Task<PayeSchemes> GetSchemesByEmployerId(long employerId);
        Task<PayeScheme> GetSchemeByRef(string empref);
    }
}
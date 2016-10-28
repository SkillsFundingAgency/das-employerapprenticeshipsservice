using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Data
{
    public interface IEmployerSchemesRepository
    {
        Task<Schemes> GetSchemesByEmployerId(long employerId);
        Task<Scheme> GetSchemeByRef(string empref);
    }
}
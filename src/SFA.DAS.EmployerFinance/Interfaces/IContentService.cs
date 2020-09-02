using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Interfaces
{
    public interface IContentService
    {
        Task<string> Get(string type, string applicationId);
    }
}

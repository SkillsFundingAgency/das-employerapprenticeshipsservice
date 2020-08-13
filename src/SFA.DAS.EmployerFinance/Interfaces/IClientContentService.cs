using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Interfaces
{
    public interface IClientContentService
    {
        Task<string> Get(string type, string applicationId);
    }
}

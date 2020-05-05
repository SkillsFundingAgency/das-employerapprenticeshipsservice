using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IClientContentService
    {
        Task<string> Get(string type, string applicationId);
    }
}

using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IContentService
    {
        Task<string> Get(string type, string applicationId);
    }
}

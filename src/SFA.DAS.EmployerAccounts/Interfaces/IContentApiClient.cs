using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IContentApiClient
    {
        Task<string> Get(string type, string applicationId);
    }
}

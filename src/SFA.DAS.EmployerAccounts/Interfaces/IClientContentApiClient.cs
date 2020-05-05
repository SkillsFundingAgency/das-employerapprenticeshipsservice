using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IClientContentApiClient
    {
        Task<string> Get(string type, string applicationId);
    }
}

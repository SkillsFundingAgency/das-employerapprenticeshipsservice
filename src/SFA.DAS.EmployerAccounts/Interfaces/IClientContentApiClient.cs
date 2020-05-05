using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IClientContentApiClient
    {
        Task<string> GetContent(string type, string clientId);
    }
}

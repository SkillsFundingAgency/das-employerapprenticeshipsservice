using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Queries.GetClientContent;

namespace SFA.DAS.EmployerAccounts.Interfaces
{
    public interface IClientContentApiClient
    {
        Task<string> GetContent(ContentType type, string clientId);
    }
}

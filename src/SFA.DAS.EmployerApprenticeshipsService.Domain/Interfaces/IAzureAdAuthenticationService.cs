using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IAzureAdAuthenticationService
    {
        Task<string> GetAuthenticationResult(string clientId, string appKey, string resourceId, string tenant);
    }
}
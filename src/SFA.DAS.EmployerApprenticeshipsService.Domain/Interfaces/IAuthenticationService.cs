using System.Threading.Tasks;

namespace SFA.DAS.EAS.Domain.Interfaces
{
    public interface IAuthenticationService
    {
        string GetClaimValue(string claimKey);
        bool IsUserAuthenticated();
        void SignOutUser();
        bool TryGetClaimValue(string key, out string value);
        Task UpdateClaims();
    }
}

using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Authentication
{
    public interface IAuthenticationService
    {
        string GetClaimValue(string claimKey);
        bool IsUserAuthenticated();
        ActionResult SignOutUser();
        bool TryGetClaimValue(string key, out string value);
        Task UpdateClaims();
    }
}

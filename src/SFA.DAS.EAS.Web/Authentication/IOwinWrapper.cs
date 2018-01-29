using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Authentication
{
    public interface IOwinWrapper
    {
        string GetClaimValue(string claimKey);
        bool IsUserAuthenticated();
        ActionResult SignOutUser();
        Task UpdateClaims();
    }
}

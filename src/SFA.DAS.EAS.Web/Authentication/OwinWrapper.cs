using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IdentityModel.Client;
using Microsoft.Owin;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Authentication
{
    public class OwinWrapper : IOwinWrapper
    {
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public OwinWrapper(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GetClaimValue(string claimKey)
        {
            var claimIdentity = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.FirstOrDefault(c => c.Type == claimKey);
            
            return claimIdentity == null ? "" : claimIdentity.Value;
            
        }

        public bool IsUserAuthenticated()
        {
            return HttpContext.Current.GetOwinContext().Authentication.User.Identity.IsAuthenticated;
        }

        public ActionResult SignOutUser()
        {
            var owinContext = HttpContext.Current.GetOwinContext();
            var authenticationManager = owinContext.Authentication;
            var idToken = authenticationManager.User.FindFirst("id_token")?.Value;
            var constants = new Constants(_configuration.Identity);

            authenticationManager.SignOut("Cookies");

            return new RedirectResult(string.Format(constants.LogoutEndpoint(), idToken, owinContext.Request.Uri.Scheme, owinContext.Request.Uri.Authority));   
        }

        public async Task UpdateClaims()
        {
            var constants = new Constants(_configuration.Identity);
            var userInfoEndpoint = constants.UserInfoEndpoint();
            var accessToken = GetClaimValue("access_token");
            var userInfoClient = new UserInfoClient(new Uri(userInfoEndpoint), accessToken);
            var userInfo = await userInfoClient.GetAsync();
            var identity = (ClaimsIdentity)HttpContext.Current.User.Identity;

            foreach (var claim in userInfo.Claims.ToList())
            {
                if (claim.Item1.Equals(DasClaimTypes.Email))
                {
                    var emailClaim = identity.Claims.FirstOrDefault(c => c.Type == "email");
                    var emailClaim2 = identity.Claims.FirstOrDefault(c => c.Type == DasClaimTypes.Email);

                    identity.RemoveClaim(emailClaim);
                    identity.RemoveClaim(emailClaim2);
                    identity.AddClaim(new Claim("email", claim.Item2));
                    identity.AddClaim(new Claim(DasClaimTypes.Email, claim.Item2));   
                }
            }
            
        }
    }
}
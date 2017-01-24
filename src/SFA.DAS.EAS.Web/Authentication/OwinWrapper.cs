using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using IdentityModel.Client;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using Microsoft.Owin;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EmployerUsers.WebClientComponents;

namespace SFA.DAS.EAS.Web.Authentication
{
    public class OwinWrapper : IOwinWrapper
    {
        private readonly IOwinContext _owinContext;
        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        public OwinWrapper(EmployerApprenticeshipsServiceConfiguration configuration)
        {
            _configuration = configuration;
            _owinContext = HttpContext.Current.GetOwinContext();
           
        }

        public void SignInUser(string id, string displayName, string email)
        {
            if (!_configuration.Identity.UseFake) { throw new NotImplementedException(); }
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, displayName),
                new Claim("email", email),
                new Claim("sub", id)
            };

            var claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            var authenticationManager = _owinContext.Authentication;
            authenticationManager.SignIn(claimsIdentity);
            _owinContext.Authentication.User = new ClaimsPrincipal(claimsIdentity);
        }

        public ActionResult SignOutUser()
        {
            var authenticationManager = _owinContext.Authentication;
            var idToken = authenticationManager.User.FindFirst("id_token")?.Value;
            authenticationManager.SignOut("Cookies");
            var constants = new Constants(_configuration.Identity);
            return new RedirectResult(string.Format(constants.LogoutEndpoint(), idToken, _owinContext.Request.Uri.Scheme, _owinContext.Request.Uri.Authority));   
        }

        public string GetClaimValue(string claimKey)
        {
            var claimIdentity = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.FirstOrDefault(claim => claim.Type == claimKey);
            
            return claimIdentity == null ? "" : claimIdentity.Value;
            
        }

        public async Task UpdateClaims()
        {
            var constants = new Constants(_configuration.Identity);
            var userInfoEndpoint = constants.UserInfoEndpoint();
            var accessToken = GetClaimValue("access_token");

            var userInfoClient = new UserInfoClient(new Uri(userInfoEndpoint), accessToken);

            var userInfo = await userInfoClient.GetAsync();

            foreach (var ui in userInfo.Claims.ToList())
            {
            
                if (ui.Item1.Equals(DasClaimTypes.Email))
                {
                    var emailClaim = ((ClaimsIdentity) HttpContext.Current.User.Identity).Claims.FirstOrDefault(
                            claim => claim.Type == "email");
                    var emailClaim2 = ((ClaimsIdentity)HttpContext.Current.User.Identity).Claims.FirstOrDefault(
                            claim => claim.Type == DasClaimTypes.Email);
                    ((ClaimsIdentity)HttpContext.Current.User.Identity).RemoveClaim(emailClaim);
                    ((ClaimsIdentity)HttpContext.Current.User.Identity).RemoveClaim(emailClaim2);

                    ((ClaimsIdentity)HttpContext.Current.User.Identity).AddClaim(new Claim("email",ui.Item2));
                    ((ClaimsIdentity)HttpContext.Current.User.Identity).AddClaim(new Claim(DasClaimTypes.Email,ui.Item2));   
                }
            }
            
        }

        public SignInMessage GetSignInMessage(string id)
        {
            return _owinContext.Environment.GetSignInMessage(id);
        }
        public void IssueLoginCookie(string id, string displayName)
        {
            //var env = _owinContext.Environment;
            //env.IssueLoginCookie(new AuthenticatedLogin
            //{
            //    Subject = id,
            //    Name = displayName
            //});
        }
        public void RemovePartialLoginCookie()
        {
            //_owinContext.Environment.RemovePartialLoginCookie();
        }
    }
}
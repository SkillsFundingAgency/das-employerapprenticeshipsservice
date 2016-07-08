using System.Collections.Generic;
using System.Security.Claims;
using IdentityServer3.Core.Extensions;
using IdentityServer3.Core.Models;
using Microsoft.Owin;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.Authentication
{
    public class OwinWrapper : IOwinWrapper
    {
        private readonly IOwinContext _owinContext;

        public OwinWrapper(IOwinContext owinContext)
        {
            _owinContext = owinContext;
        }

        public void SignInUser(string id, string displayName, string email)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, displayName),
                new Claim(ClaimTypes.Email, email),
                new Claim("sub", id)
            };

            var claimsIdentity = new ClaimsIdentity(claims,"Cookies");

            var authenticationManager = _owinContext.Authentication;
            authenticationManager.SignIn(claimsIdentity);
            _owinContext.Authentication.User = new ClaimsPrincipal(claimsIdentity);
        }

        public void SignOutUser()
        {
            var authenticationManager = _owinContext.Authentication;
            authenticationManager.SignOut("Cookies");
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
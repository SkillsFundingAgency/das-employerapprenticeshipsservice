using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Infrastructure.Authentication;

namespace SFA.DAS.EAS.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Method)]
    public class SignOutAttribute : ActionFilterAttribute
    {
        private readonly Func<IAuthenticationService> _authenticationService;

        public SignOutAttribute()
            : this(() => DependencyResolver.Current.GetService<IAuthenticationService>())
        {
        }

        public SignOutAttribute(Func<IAuthenticationService> authenticationService)
        {
            _authenticationService = authenticationService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _authenticationService().SignOutUser();
        }
    }
}
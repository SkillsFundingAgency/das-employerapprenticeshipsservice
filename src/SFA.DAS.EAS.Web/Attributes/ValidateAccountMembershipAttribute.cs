using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Services;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Helpers;

namespace SFA.DAS.EAS.Web.Attributes
{
    public class ValidateAccountMembershipAttribute : ActionFilterAttribute
    {
        private readonly Func<ICurrentUserService> _currentUserService;
        private readonly Func<IMembershipService> _membershipService;

        public ValidateAccountMembershipAttribute()
            : this(() => DependencyResolver.Current.GetService<ICurrentUserService>(), () => DependencyResolver.Current.GetService<IMembershipService>())
        {
        }

        public ValidateAccountMembershipAttribute(Func<ICurrentUserService> currentUserService, Func<IMembershipService> membershipService)
        {
            _currentUserService = currentUserService;
            _membershipService = membershipService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var accountHashedId = filterContext.RouteData.Values[ControllerConstants.HashedAccountIdKeyName].ToString();
            var currentUser = _currentUserService().GetCurrentUser();

            _membershipService().ValidateAccountMembership(accountHashedId, currentUser.ExternalId);
        }
    }
}
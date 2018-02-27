using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Web.Authorization;

namespace SFA.DAS.EAS.Web.Attributes
{
    public class ValidateMembershipAttribute : ActionFilterAttribute
    {
        private readonly Func<IMembershipService> _membershipService;

        public ValidateMembershipAttribute()
            : this(() => DependencyResolver.Current.GetService<IMembershipService>())
        {
        }

        public ValidateMembershipAttribute(Func<IMembershipService> membershipService)
        {
            _membershipService = membershipService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _membershipService().ValidateMembership();
        }
    }
}
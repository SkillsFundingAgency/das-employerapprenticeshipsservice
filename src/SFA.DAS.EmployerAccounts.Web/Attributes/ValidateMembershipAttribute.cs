﻿using System;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Authorization;

namespace SFA.DAS.EmployerAccounts.Web.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class ValidateMembershipAttribute : ActionFilterAttribute
    {
        private readonly Func<IAuthorizationService> _authorizationService;

        public ValidateMembershipAttribute()
            : this(() => DependencyResolver.Current.GetService<IAuthorizationService>())
        {
        }

        public ValidateMembershipAttribute(Func<IAuthorizationService> authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            _authorizationService().ValidateMembership();
        }
    }
}
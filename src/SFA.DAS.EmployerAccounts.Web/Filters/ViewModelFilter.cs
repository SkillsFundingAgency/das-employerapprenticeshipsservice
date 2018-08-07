﻿using System;
using System.Web.Mvc;
using SFA.DAS.EmployerAccounts.Authorization;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.Filters
{
    public class ViewModelFilter : ActionFilterAttribute
    {
        private readonly Func<IAuthorizationService> _authorizationService;

        public ViewModelFilter(Func<IAuthorizationService> authorizationService)
        {
            _authorizationService = authorizationService;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Controller.ViewData.Model is IAccountViewModel viewModel)
            {
                var authorizationContext = _authorizationService().GetAuthorizationContext();

                viewModel.AccountId = authorizationContext.AccountContext.Id;
                viewModel.AccountHashedId = authorizationContext.AccountContext.HashedId;
            }
        }
    }
}
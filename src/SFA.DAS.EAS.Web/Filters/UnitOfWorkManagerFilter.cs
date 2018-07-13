﻿using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Infrastructure.Data;

namespace SFA.DAS.EAS.Web.Filters
{
    public class UnitOfWorkManagerFilter : ActionFilterAttribute
    {
        private readonly Func<IUnitOfWorkManagerAccount> _unitOfWorkManager;

        public UnitOfWorkManagerFilter(Func<IUnitOfWorkManagerAccount> unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!filterContext.IsChildAction && filterContext.Exception == null)
            {
                _unitOfWorkManager().End();
            }
        }
    }
}
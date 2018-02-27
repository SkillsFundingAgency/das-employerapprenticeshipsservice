using System;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Data;

namespace SFA.DAS.EAS.Web.Filters
{
    public class UnitOfWorkManagerFilter : ActionFilterAttribute
    {
        private readonly Func<IUnitOfWorkManager> _unitOfWorkManager;

        public UnitOfWorkManagerFilter(Func<IUnitOfWorkManager> unitOfWorkManager)
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
using System;
using System.Web.Mvc;
using SFA.DAS.EmployerFinance.Data;

namespace SFA.DAS.EmployerFinance.Web.Filters
{
    public class UnitOfWorkManagerFilter : ActionFilterAttribute
    {
        private readonly Func<IUnitOfWorkManager> _unitOfWorkManager;

        public UnitOfWorkManagerFilter(Func<IUnitOfWorkManager> unitOfWorkManager)
        {
            _unitOfWorkManager = unitOfWorkManager;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (!filterContext.IsChildAction)
            {
                _unitOfWorkManager().Begin();
            }
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
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Web.Filters
{
    public class ViewModelFilter : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewModel = filterContext.Controller.ViewData.Model as ViewModel;

            if (viewModel != null)
            {
                viewModel.OnActionExecuted();
            }
        }
    }
}
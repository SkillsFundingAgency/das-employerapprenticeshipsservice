using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Application.Messages;

namespace SFA.DAS.EAS.Web.Filters
{
    public class MapViewModelToMessageFilter : ActionFilterAttribute
    {
        private readonly IMapper _mapper;

        public MapViewModelToMessageFilter(IMapper mapper)
        {
            _mapper = mapper;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewModel = filterContext.Controller.ViewData.Model as ViewModel;

            if (viewModel != null)
            {
                viewModel.Map(_mapper);
            }
        }
    }
}
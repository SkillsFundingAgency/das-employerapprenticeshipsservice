using System;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Filters
{
    public class MapViewModelToMessageFilter : ActionFilterAttribute
    {
        private readonly Func<IMapper> _mapper;

        public MapViewModelToMessageFilter(Func<IMapper> mapper)
        {
            _mapper = mapper;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (filterContext.Controller.ViewData.Model is ViewModel viewModel)
            {
                viewModel.Map(_mapper());
            }
        }
    }
}
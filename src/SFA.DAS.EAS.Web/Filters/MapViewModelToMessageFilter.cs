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
            var viewModel = filterContext.Controller.ViewData.Model as ViewModel;

            if (viewModel != null)
            {
                viewModel.Map(_mapper());
            }
        }
    }
}
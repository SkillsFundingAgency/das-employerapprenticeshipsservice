using System;
using System.Web.Mvc;
using AutoMapper;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Filters
{
    public class MapViewModelToMessageFilter : ActionFilterAttribute
    {
        private readonly Func<IMapper> _mapperFactory;

        public MapViewModelToMessageFilter(Func<IMapper> mapperFactory)
        {
            _mapperFactory = mapperFactory;
        }

        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            var viewModel = filterContext.Controller.ViewData.Model as ViewModel;

            if (viewModel != null)
            {
                viewModel.Map(_mapperFactory());
            }
        }
    }
}
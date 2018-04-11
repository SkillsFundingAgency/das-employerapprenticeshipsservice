using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetActivities;
using SFA.DAS.EAS.Application.Queries.GetLatestActivities;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Features;
using SFA.DAS.EAS.Web.Attributes;
using SFA.DAS.EAS.Web.Helpers;
using SFA.DAS.EAS.Web.ViewModels.Activities;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [Feature(FeatureType.Activities)]
    [ValidateMembership]
    [RoutePrefix("accounts/{HashedAccountId}/activity")]
    public class ActivitiesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly ILog _logger;

        public ActivitiesController(IMapper mapper, IMediator mediator, ILog logger)
        {
            _mapper = mapper;
            _mediator = mediator;
            _logger = logger;
        }
        
        [Route]
        public async Task<ActionResult> Index(GetActivitiesQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<ActivitiesViewModel>(response);

            return View(model);
        }

        [ChildActionOnly]
        public ActionResult Latest(GetLatestActivitiesQuery query)
        {
            try
            {
                var response = Task.Run(async () => await _mediator.SendAsync(query)).Result;
                var model = _mapper.Map<LatestActivitiesViewModel>(response);

                return PartialView(model);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Failed to get the latest activities.");

                return Content(ControllerConstants.ActivitiesUnavailableMessage);
            }
        }
    }
}
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetActivities;
using SFA.DAS.EAS.Application.Queries.GetLatestActivities;
using SFA.DAS.EAS.Web.Attributes;
using SFA.DAS.EAS.Web.ViewModels.Activities;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
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
        [Route("latest")]
        public ActionResult Latest(GetLatestActivitiesQuery query)
        {
            try
            {
                var response = Task.Run(async () => await _mediator.SendAsync(query)).Result; 

                var model = _mapper.Map<LatestActivitiesViewModel>(response);

                return PartialView(model);
            }
            catch (Exception e)
            {
                _logger.Warn($"Failed to get the latest activities: {e.GetType().Name} - {e.Message}");
                return Content(ActivitiesUnavailableMessage);
            }
        }

        public const string ActivitiesUnavailableMessage = "Activities are currently unavailable.";
    }
}
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetActivities;
using SFA.DAS.EAS.Application.Queries.GetLatestActivities;
using SFA.DAS.EAS.Web.Attributes;
using SFA.DAS.EAS.Web.ViewModels.Activities;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [ValidateMembership]
    [RoutePrefix("accounts/{HashedAccountId}/activity")]
    public class ActivitiesController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public ActivitiesController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
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
            var response = Task.Run(async () => await _mediator.SendAsync(query)).Result;
            var model = _mapper.Map<LatestActivitiesViewModel>(response);

            return PartialView(model);
        }
    }
}
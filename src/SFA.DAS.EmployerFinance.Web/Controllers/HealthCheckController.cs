using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerFinance.Commands.RunHealthCheckCommand;
using SFA.DAS.EmployerFinance.Queries.GetHealthCheck;
using SFA.DAS.EmployerFinance.Web.ViewModels;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [Authorize]
    [RoutePrefix("healthcheck")]
    public class HealthCheckController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public HealthCheckController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [Route]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index(GetHealthCheckQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<HealthCheckViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Index(RunHealthCheckCommand command)
        {
            await _mediator.SendAsync(command);

            return RedirectToAction("Index");
        }
    }
}
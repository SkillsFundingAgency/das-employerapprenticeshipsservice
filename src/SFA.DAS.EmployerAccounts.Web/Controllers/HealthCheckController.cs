using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SFA.DAS.EmployerAccounts.Commands.RunHealthCheckCommand;
using SFA.DAS.EmployerAccounts.Queries.GetHealthCheck;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [RoutePrefix("healthcheck")]
    public class HealthCheckController : Controller
    {
        private readonly IMediator _mediator;

        public HealthCheckController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route]
        public async Task<ActionResult> Index(GetHealthCheckQuery query)
        {
            var response = await _mediator.SendAsync(query);

            return View(response.HealthCheck);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Route]
        public async Task<ActionResult> Index(RunHealthCheckCommand command)
        {
            await _mediator.SendAsync(command);

            return RedirectToAction("Index");
        }
    }
}
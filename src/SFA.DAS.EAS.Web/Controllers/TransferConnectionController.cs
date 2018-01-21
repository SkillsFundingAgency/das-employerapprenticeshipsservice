using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetCreatedTransferConnection;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionQuery;
using SFA.DAS.EAS.Web.Attributes;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}/transfers/connections")]
    public class TransferConnectionController : Controller
    {
        private readonly IMediator _mediator;

        public TransferConnectionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
        
        [Route("create")]
        public ActionResult Create(string hashedAccountId)
        {
            return View(new CreateTransferConnectionViewModel
            {
                SenderHashedAccountId = hashedAccountId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("create")]
        public async Task<ActionResult> Create(CreateTransferConnectionViewModel model)
        {
            var transferConnectionId = await _mediator.SendAsync(model.Message);

            return RedirectToAction("Send", new GetCreatedTransferConnectionQuery { TransferConnectionId = transferConnectionId });
        }

        [HttpNotFoundForNullModel]
        [Route("{transferConnectionId}/send")]
        public async Task<ActionResult> Send(GetCreatedTransferConnectionQuery query)
        {
            var model = await _mediator.SendAsync(query);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionId}/send")]
        public async Task<ActionResult> Send(CreatedTransferConnectionViewModel model)
        {
            switch (model.Choice)
            {
                case 1:
                    await _mediator.SendAsync(model.Message);
                    return RedirectToAction("Sent", new GetSentTransferConnectionQuery { TransferConnectionId = model.Message.TransferConnectionId });
                default:
                    return RedirectToAction("Index", "Transfer");
            }
        }

        [HttpNotFoundForNullModel]
        [Route("{transferConnectionId}/sent")]
        public async Task<ActionResult> Sent(GetSentTransferConnectionQuery query)
        {
            var model = await _mediator.SendAsync(query);

            return View(model);
        }

        [HttpPost]
        [Route("{transferConnectionId}/sent")]
        public ActionResult Sent(SentTransferConnectionViewModel model)
        {
            switch (model.Choice)
            {
                case 1:
                    return RedirectToAction("Index", "Transfer");
                default:
                    return RedirectToAction("Index", "EmployerTeam");
            }
        }
    }
}
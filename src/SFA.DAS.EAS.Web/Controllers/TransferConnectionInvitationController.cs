using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetCreatedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitationQuery;
using SFA.DAS.EAS.Web.Attributes;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitation;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [RoutePrefix("accounts/{hashedAccountId}/transfers/connections/invitations")]
    public class TransferConnectionInvitationController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public TransferConnectionInvitationController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [Route("")]
        public ActionResult Index()
        {
            return View();
        }
        
        [ImportModelStateFromTempData]
        [Route("create")]
        public ActionResult Create(string hashedAccountId)
        {
            return View(new CreateTransferConnectionInvitationViewModel
            {
                SenderHashedAccountId = hashedAccountId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("create")]
        public async Task<ActionResult> Create(CreateTransferConnectionInvitationViewModel model)
        {
            var transferConnectionInvitationId = await _mediator.SendAsync(model.Message);

            return RedirectToAction("Send", new GetCreatedTransferConnectionInvitationQuery { TransferConnectionInvitationId = transferConnectionInvitationId });
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/send")]
        public async Task<ActionResult> Send(GetCreatedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<CreatedTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/send")]
        public async Task<ActionResult> Send(CreatedTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "Confirm":
                    await _mediator.SendAsync(model.Message);
                    return RedirectToAction("Sent", new GetSentTransferConnectionInvitationQuery { TransferConnectionInvitationId = model.Message.TransferConnectionInvitationId });
                case "GoToTransfersPage":
                    return RedirectToAction("Index", "Transfer");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/sent")]
        public async Task<ActionResult> Sent(GetSentTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<SentTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/sent")]
        public ActionResult Sent(SentTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "GoToTransfersPage":
                    return RedirectToAction("Index", "Transfer");
                case "GoToHomepage":
                    return RedirectToAction("Index", "EmployerTeam");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }
    }
}
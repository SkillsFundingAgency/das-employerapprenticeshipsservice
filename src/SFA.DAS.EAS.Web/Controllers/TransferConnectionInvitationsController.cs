using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetApprovedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetLatestPendingReceivedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetReceivedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetRejectedTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitation;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAccount;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionRoles;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Features;
using SFA.DAS.EAS.Web.Attributes;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [Feature(FeatureType.Transfers)]
    [ValidateMembership]
    [RoutePrefix("accounts/{HashedAccountId}/transfers/connections/invitations")]
    public class TransferConnectionInvitationsController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public TransferConnectionInvitationsController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [Route]
        public async Task<ActionResult> Index(GetTransferConnectionRolesQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<TransferConnectionRolesViewModel>(response);

            return View(model);
        }
        
        [ImportModelStateFromTempData]
        [Route("start")]
        public ActionResult Start()
        {
            return View(new StartTransferConnectionInvitationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("start")]
        public async Task<ActionResult> Start(StartTransferConnectionInvitationViewModel model)
        {
            await _mediator.SendAsync(model.GetTransferConnectionInvitationAccountQuery);
            return RedirectToAction("Send", new { receiverAccountPublicHashedId = model.GetTransferConnectionInvitationAccountQuery.ReceiverAccountPublicHashedId });
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("send")]
        public async Task<ActionResult> Send(GetTransferConnectionInvitationAccountQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<SendTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("send")]
        public async Task<ActionResult> Send(SendTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "Confirm":
                    var transferConnectionInvitationId = await _mediator.SendAsync(model.SendTransferConnectionInvitationCommand);
                    return RedirectToAction("Sent", new { transferConnectionInvitationId });
                case "GoToTransfersPage":
                    return RedirectToAction("Index", "Transfers");
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
                    return RedirectToAction("Index", "Transfers");
                case "GoToHomepage":
                    return RedirectToAction("Index", "EmployerTeam");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("receive")]
        public async Task<ActionResult> Receive(GetReceivedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<ReceiveTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("receive")]
        public async Task<ActionResult> Receive(ReceiveTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "Approve":
                    await _mediator.SendAsync(model.ApproveTransferConnectionInvitationCommand);
                    return RedirectToAction("Approved", new { transferConnectionInvitationId = model.ApproveTransferConnectionInvitationCommand.TransferConnectionInvitationId });
                case "Reject":
                    await _mediator.SendAsync(model.RejectTransferConnectionInvitationCommand);
                    return RedirectToAction("Rejected", new { transferConnectionInvitationId = model.RejectTransferConnectionInvitationCommand.TransferConnectionInvitationId });
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/approved")]
        public async Task<ActionResult> Approved(GetApprovedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<ApprovedTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/approved")]
        public ActionResult Approved(ApprovedTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "GoToApprenticesPage":
                    return Redirect(Url.CommitmentsAction("apprentices/home"));
                case "GoToHomepage":
                    return RedirectToAction("Index", "EmployerTeam");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/rejected")]
        public async Task<ActionResult> Rejected(GetRejectedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<RejectedTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/rejected")]
        public ActionResult Rejected(RejectedTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "GoToTransfersPage":
                    return RedirectToAction("Index", "Transfers");
                case "GoToHomepage":
                    return RedirectToAction("Index", "EmployerTeam");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpNotFoundForNullModel]
        [Route("{transferConnectionInvitationId}/details")]
        public async Task<ActionResult> Details(GetTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<TransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/details")]
        public async Task<ActionResult> Details(TransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "Confirm":
                    await _mediator.SendAsync(model.DeleteTransferConnectionInvitationCommand);
                    return RedirectToAction("Deleted");
                case "GoToTransfersPage":
                    return RedirectToAction("Index", "Transfers");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/deleted")]
        public ActionResult Deleted()
        {
            var model = new DeletedTransferConnectionInvitationViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/deleted")]
        public ActionResult Deleted(DeletedTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "GoToTransferDashboard":
                    return RedirectToAction("Index", "Transfers");
                case "GoToHomepage":
                    return RedirectToAction("Index", "EmployerTeam");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpGet]
        [Route("outstanding")]
        public async Task<ActionResult> Outstanding(GetLatestPendingReceivedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);

            return response.TransferConnectionInvitation == null
                ? RedirectToAction("Index", "Transfers")
                : RedirectToAction("Receive", new { transferConnectionInvitationId = response.TransferConnectionInvitation.Id });
        }
    }
}
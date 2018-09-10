﻿using AutoMapper;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EmployerFinance.Queries.GetApprovedTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetLatestPendingReceivedTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetReceivedTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetRejectedTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.SendTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Web.Extensions;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using System;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [Authorize]
    [Feature(FeatureType.TransferConnectionRequests)]
    [ValidateMembership]
    [RoutePrefix("accounts/{HashedAccountId}/transfers/connections/requests")]
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
        public ActionResult Index()
        {
            return View();
        }

        // [ImportModelStateFromTempData]
        [Route("start")]
        public ActionResult Start()
        {
            return View(new StartTransferConnectionInvitationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [ValidateModelState]
        [Route("start")]
        public async Task<ActionResult> Start(StartTransferConnectionInvitationViewModel model)
        {
            await _mediator.SendAsync(model.SendTransferConnectionInvitationQuery);
            return RedirectToAction("Send", new { receiverAccountPublicHashedId = model.SendTransferConnectionInvitationQuery.ReceiverAccountPublicHashedId });
        }

        // [HttpNotFoundForNullModel]
        // [ImportModelStateFromTempData]
        [Route("send")]
        public async Task<ActionResult> Send(SendTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<SendTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [ValidateModelState]
        [Route("send")]
        public async Task<ActionResult> Send(SendTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "Confirm":
                    var transferConnectionInvitationId = await _mediator.SendAsync(model.SendTransferConnectionInvitationCommand);
                    return RedirectToAction("Sent", new { transferConnectionInvitationId });
                case "ReEnterAccountId":
                    return RedirectToAction("Start");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        // [HttpNotFoundForNullModel]
        // [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/sent")]
        public async Task<ActionResult> Sent(GetSentTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<SentTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [ValidateModelState]
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

        // [HttpNotFoundForNullModel]
        // [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/receive")]
        public async Task<ActionResult> Receive(GetReceivedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<ReceiveTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [ValidateModelState]
        [Route("{transferConnectionInvitationId}/receive")]
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

        // [HttpNotFoundForNullModel]
        //  [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/approved")]
        public async Task<ActionResult> Approved(GetApprovedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<ApprovedTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [ValidateModelState]
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

        //[HttpNotFoundForNullModel]
        // [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/rejected")]
        public async Task<ActionResult> Rejected(GetRejectedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<RejectedTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [ValidateModelState]
        [Route("{transferConnectionInvitationId}/rejected")]
        public async Task<ActionResult> Rejected(RejectedTransferConnectionInvitationViewModel model)
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

        // [HttpNotFoundForNullModel]
        [Route("{transferConnectionInvitationId}/details")]
        public async Task<ActionResult> Details(GetTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<TransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        //[ValidateModelState]
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

        // [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/deleted")]
        public ActionResult Deleted()
        {
            var model = new DeletedTransferConnectionInvitationViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        // [ValidateModelState]
        [Route("{transferConnectionInvitationId}/deleted")]
        public ActionResult Deleted(DeletedTransferConnectionInvitationViewModel model)
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
using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.EmployerFinance.Commands.ApproveTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Commands.DeleteSentTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Commands.RejectTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Commands.SendTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetApprovedTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetLatestPendingReceivedTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetReceivedTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetRejectedTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetSentTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Queries.SendTransferConnectionInvitation;
using SFA.DAS.EmployerFinance.Web.Extensions;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using SFA.DAS.Validation.Mvc;

namespace SFA.DAS.EmployerFinance.Web.Controllers
{
    [DasAuthorize("EmployerFeature.TransferConnectionRequests", EmployerUserRole.Any)]
    [RoutePrefix("accounts/{HashedAccountId}/transfers/connections/requests")]
    public class TransferConnectionInvitationsController : Microsoft.AspNetCore.Mvc.Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public TransferConnectionInvitationsController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
        }

        [Route]
        public Microsoft.AspNetCore.Mvc.ActionResult Index()
        {
            return View();
        }

        [ImportModelStateFromTempData]
        [Route("start")]
        public Microsoft.AspNetCore.Mvc.ActionResult Start()
        {
            return View(new StartTransferConnectionInvitationViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("start")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Start(StartTransferConnectionInvitationViewModel model)
        {
            await _mediator.SendAsync(new SendTransferConnectionInvitationQuery
            {
                ReceiverAccountPublicHashedId = model.ReceiverAccountPublicHashedId,
                AccountId = model.AccountId
            });
            return RedirectToAction("Send", new { receiverAccountPublicHashedId = model.ReceiverAccountPublicHashedId });
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("send")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Send(SendTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<SendTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("send")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Send(SendTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "Confirm":
                    var transferConnectionInvitationId = await _mediator.SendAsync(new SendTransferConnectionInvitationCommand
                    {
                        AccountId = model.AccountId,
                        ReceiverAccountPublicHashedId = model.ReceiverAccountPublicHashedId,
                        UserRef = model.UserRef
                    });
                    return RedirectToAction("Sent", new { transferConnectionInvitationId });
                case "ReEnterAccountId":
                    return RedirectToAction("Start");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/sent")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Sent(GetSentTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<SentTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/sent")]
        public Microsoft.AspNetCore.Mvc.ActionResult Sent(SentTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "GoToTransfersPage":
                    return RedirectToAction("Index", "TransferConnections");
                case "GoToHomepage":
                    return Redirect(Url.EmployerAccountsAction("teams"));
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/receive")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Receive(GetReceivedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<ReceiveTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/receive")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Receive(ReceiveTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "Approve":
                    await _mediator.SendAsync(new ApproveTransferConnectionInvitationCommand { AccountId = model.AccountId, UserRef = model.UserRef, TransferConnectionInvitationId = model.TransferConnectionInvitationId });
                    return RedirectToAction("Approved", new { transferConnectionInvitationId = model.TransferConnectionInvitationId });
                case "Reject":
                    await _mediator.SendAsync(new RejectTransferConnectionInvitationCommand { AccountId = model.AccountId, UserRef = model.UserRef, TransferConnectionInvitationId = model.TransferConnectionInvitationId });
                    return RedirectToAction("Rejected", new { transferConnectionInvitationId = model.TransferConnectionInvitationId });
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/approved")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Approved(GetApprovedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<ApprovedTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/approved")]
        public Microsoft.AspNetCore.Mvc.ActionResult Approved(ApprovedTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "GoToApprenticesPage":
                    return Redirect(Url.EmployerCommitmentsV2Action(string.Empty));
                case "GoToHomepage":
                    return Redirect(Url.EmployerAccountsAction("teams"));
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpNotFoundForNullModel]
        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/rejected")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Rejected(GetRejectedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<RejectedTransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/rejected")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Rejected(RejectedTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "Confirm":
                    await _mediator.SendAsync(new DeleteTransferConnectionInvitationCommand
                    {
                        AccountId = model.AccountId,
                        TransferConnectionInvitationId = model.TransferConnectionInvitationId,
                        UserRef = model.UserRef
                    });
                    return RedirectToAction("Deleted");
                case "GoToTransfersPage":
                    return RedirectToAction("Index", "TransferConnections");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpNotFoundForNullModel]
        [Route("{transferConnectionInvitationId}/details")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Details(GetTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<TransferConnectionInvitationViewModel>(response);

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/details")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Details(TransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "Confirm":
                    await _mediator.SendAsync(new DeleteTransferConnectionInvitationCommand
                    {
                        AccountId = model.AccountId,
                        TransferConnectionInvitationId = model.TransferConnectionInvitationId,
                        UserRef = model.UserRef
                    });
                    return RedirectToAction("Deleted");
                case "GoToTransfersPage":
                    return RedirectToAction("Index", "TransferConnections");
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [ImportModelStateFromTempData]
        [Route("{transferConnectionInvitationId}/deleted")]
        public Microsoft.AspNetCore.Mvc.ActionResult Deleted()
        {
            var model = new DeletedTransferConnectionInvitationViewModel();

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateModelState]
        [Route("{transferConnectionInvitationId}/deleted")]
        public Microsoft.AspNetCore.Mvc.ActionResult Deleted(DeletedTransferConnectionInvitationViewModel model)
        {
            switch (model.Choice)
            {
                case "GoToTransfersPage":
                    return RedirectToAction("Index", "TransferConnections");
                case "GoToHomepage":
                    return Redirect(Url.EmployerAccountsAction("teams"));
                default:
                    throw new ArgumentOutOfRangeException(nameof(model.Choice));
            }
        }

        [HttpGet]
        [Route("outstanding")]
        public async Task<Microsoft.AspNetCore.Mvc.ActionResult> Outstanding(GetLatestPendingReceivedTransferConnectionInvitationQuery query)
        {
            var response = await _mediator.SendAsync(query);

            return response.TransferConnectionInvitation == null
                ? RedirectToAction("Index", "TransferConnections")
                : RedirectToAction("Receive", new { transferConnectionInvitationId = response.TransferConnectionInvitation.Id });
        }
    }
}
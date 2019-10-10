using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Mvc.Attributes;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.EmployerAccounts.Queries.GetTransferAllowance;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitationAuthorization;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EmployerAccounts.Queries.GetTransferRequests;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [DasAuthorize(EmployerUserRole.Any)]
    [RoutePrefix("accounts/{HashedAccountId}/transfers")]
    public class TransfersController : Controller
    {
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public TransfersController(ILog logger, IMapper mapper, IMediator mediator)
        {
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
        }

        [Route]
        public async Task<ActionResult> Index(GetEmployerAccountDetailByHashedIdQuery query)
        {
            var response = await _mediator.SendAsync(query);
            ViewBag.ApprenticeshipEmployerType = response.Account.ApprenticeshipEmployerType;
            return View();
        }

        [ChildActionOnly]
        public ActionResult TransferAllowance(GetTransferAllowanceQuery query)
        {
            var response = Task.Run(() => _mediator.SendAsync(query)).GetAwaiter().GetResult();
            var model = _mapper.Map<TransferAllowanceViewModel>(response);

            return PartialView(model);
        }

        [ChildActionOnly]
        [DasAuthorize("EmployerFeature.Transfers")]
        public ActionResult TransferConnectionInvitationAuthorization(GetTransferConnectionInvitationAuthorizationQuery query)
        {
            var response = Task.Run(() => _mediator.SendAsync(query)).GetAwaiter().GetResult();
            var model = _mapper.Map<TransferConnectionInvitationAuthorizationViewModel>(response);

            return PartialView(model);
        }

        [ChildActionOnly]
        [DasAuthorize("EmployerFeature.Transfers")]
        public ActionResult TransferConnectionInvitations(GetTransferConnectionInvitationsQuery query)
        {
            var response = Task.Run(() => _mediator.SendAsync(query)).GetAwaiter().GetResult();
            var model = _mapper.Map<TransferConnectionInvitationsViewModel>(response);

            return PartialView(model);
        }

        [ChildActionOnly]
        [DasAuthorize("EmployerFeature.Transfers")]
        public ActionResult TransferRequests(GetTransferRequestsQuery query)
        {
            try
            {
                var response = Task.Run(() => _mediator.SendAsync(query)).GetAwaiter().GetResult();
                var model = _mapper.Map<TransferRequestsViewModel>(response);

                return PartialView(model);
            }
            catch (Exception ex)
            {
                _logger.Warn(ex, "Failed to get transfer requests");

                return new EmptyResult();
            }
        }
    }
}
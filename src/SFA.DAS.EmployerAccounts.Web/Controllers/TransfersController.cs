using System;
using System.Threading.Tasks;
using System.Web.Mvc;
using AutoMapper;
using MediatR;
using SFA.DAS.Authorization;
using SFA.DAS.Authorization.Mvc;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Queries.GetTransferAllowance;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitationAuthorization;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EmployerAccounts.Queries.GetTransferRequests;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Web.Controllers
{
    [Authorize]
    [ValidateMembership]
    [RoutePrefix("accounts/{HashedAccountId}/transfers")]
    public class TransfersController : Controller
    {
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;
        private readonly EmployerFinanceConfiguration _configuration;

        public TransfersController(ILog logger, IMapper mapper, IMediator mediator,EmployerFinanceConfiguration configuration)
        {
            _logger = logger;
            _mapper = mapper;
            _mediator = mediator;
            _configuration = configuration;
        }

        [Route]
        public ActionResult Index()
        {
            return View();
        }

        [ChildActionOnly]
        public ActionResult TransferAllowance(GetTransferAllowanceQuery query)
        {
            var response = Task.Run(() => _mediator.SendAsync(query)).GetAwaiter().GetResult();
            var model = _mapper.Map<TransferAllowanceViewModel>(response);
            model.PercentLevyTransferAllowance = _configuration.TransferAllowancePercentage;

            return PartialView(model);
        }

        [ChildActionOnly]
        [Feature(FeatureType.Transfers)]
        public ActionResult TransferConnectionInvitationAuthorization(GetTransferConnectionInvitationAuthorizationQuery query)
        {
            var response = Task.Run(() => _mediator.SendAsync(query)).GetAwaiter().GetResult();
            var model = _mapper.Map<TransferConnectionInvitationAuthorizationViewModel>(response);
            model.PercentLevyTransferAllowance = _configuration.TransferAllowancePercentage;

            return PartialView(model);
        }

        [ChildActionOnly]
        [Feature(FeatureType.Transfers)]
        public ActionResult TransferConnectionInvitations(GetTransferConnectionInvitationsQuery query)
        {
            var response = Task.Run(() => _mediator.SendAsync(query)).GetAwaiter().GetResult();
            var model = _mapper.Map<TransferConnectionInvitationsViewModel>(response);

            return PartialView(model);
        }

        [ChildActionOnly]
        [Feature(FeatureType.Transfers)]
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
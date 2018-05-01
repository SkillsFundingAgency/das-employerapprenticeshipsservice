using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EAS.Web.Attributes;
using SFA.DAS.EAS.Web.ViewModels.Transfers;
using System.Threading.Tasks;
using System.Web.Mvc;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitationAuthorization;
using SFA.DAS.EAS.Application.Queries.GetTransferRequests;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Features;
using SFA.DAS.EAS.Web.ViewModels.TransferConnectionInvitations;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
    [Feature(FeatureType.Transfers)]
    [ValidateMembership]
    [RoutePrefix("accounts/{HashedAccountId}/transfers")]
    public class TransfersController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IMediator _mediator;

        public TransfersController(IMapper mapper, IMediator mediator)
        {
            _mapper = mapper;
            _mediator = mediator;
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

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult TransferConnectionInvitationAuthorization(GetTransferConnectionInvitationAuthorizationQuery query)
        {
            var response = Task.Run(() => _mediator.SendAsync(query)).GetAwaiter().GetResult();
            var model = _mapper.Map<TransferConnectionInvitationAuthorizationViewModel>(response);

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult TransferConnectionInvitations(GetTransferConnectionInvitationsQuery query)
        {
            var response = Task.Run(() => _mediator.SendAsync(query)).GetAwaiter().GetResult();
            var model = _mapper.Map<TransferConnectionInvitationsViewModel>(response);

            return PartialView(model);
        }

        [ChildActionOnly]
        public ActionResult TransferRequests(GetTransferRequestsQuery query)
        {
            var response = Task.Run(() => _mediator.SendAsync(query)).GetAwaiter().GetResult();
            var model = _mapper.Map<TransferRequestsViewModel>(response);

            return PartialView(model);
        }
    }
}
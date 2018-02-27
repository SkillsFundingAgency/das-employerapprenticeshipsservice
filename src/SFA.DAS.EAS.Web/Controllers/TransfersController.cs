using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Application.Queries.GetTransferConnectionInvitations;
using SFA.DAS.EAS.Web.Attributes;
using SFA.DAS.EAS.Web.ViewModels.Transfers;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.Controllers
{
    [Authorize]
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
        public async Task<ActionResult> Index(GetTransferConnectionInvitationsQuery query)
        {
            var response = await _mediator.SendAsync(query);
            var model = _mapper.Map<TransferConnectionInvitationsViewModel>(response);

            return View(model);
        }
    }
}
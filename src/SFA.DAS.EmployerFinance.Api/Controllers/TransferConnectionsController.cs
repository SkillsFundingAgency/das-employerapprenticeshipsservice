using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EmployerFinance.Api.Attributes;
using SFA.DAS.EmployerFinance.Queries.GetTransferConnections;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerFinance.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accounts")]
    public class TransferConnectionsController : ApiController
    {
        private readonly IMediator _mediator;
        private readonly IHashingService _hashingService;

        public TransferConnectionsController(IMediator mediator, IHashingService hashingService)
        {
            _mediator = mediator;
            _hashingService = hashingService;
        }

        [Route("{hashedAccountId}/transfers/connections")]
        public async Task<IHttpActionResult> GetTransferConnections(string hashedAccountId)
        {
            var accountId = _hashingService.DecodeValue(hashedAccountId);

            var response = await _mediator.SendAsync(new GetTransferConnectionsQuery { AccountId = accountId });
            return Ok(response.TransferConnections);
        }

        [Route("internal/{accountId}/transfers/connections")]
        public async Task<IHttpActionResult> GetTransferConnections(long accountId)
        {
            var response = await _mediator.SendAsync(new GetTransferConnectionsQuery { AccountId = accountId });
            return Ok(response.TransferConnections);
        }
    }
}
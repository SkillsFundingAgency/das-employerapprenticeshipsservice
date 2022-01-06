using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnections;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
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
            var response = await _mediator.SendAsync( new GetTransferConnectionsQuery{ HashedAccountId = hashedAccountId});
            return Ok(response.TransferConnections);
        }

        [Route("internal/{accountId}/transfers/connections")]
        public async Task<IHttpActionResult> GetTransferConnections(long accountId)
        {
            var hashedAccountId = _hashingService.HashValue(accountId);

            var response = await _mediator.SendAsync(new GetTransferConnectionsQuery { HashedAccountId = hashedAccountId });
            return Ok(response.TransferConnections);
        }
    }
}
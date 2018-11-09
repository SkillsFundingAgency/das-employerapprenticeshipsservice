using System.Threading.Tasks;
using System.Web.Http;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Attributes;
using SFA.DAS.EmployerAccounts.Queries.GetTransferConnections;

namespace SFA.DAS.EmployerAccounts.Api.Controllers
{
    [ApiAuthorize(Roles = "ReadUserAccounts")]
    [RoutePrefix("api/accounts/{hashedAccountId}/transfers/connections")]
    public class TransferConnectionsController : ApiController
    {
        private readonly IMediator _mediator;

        public TransferConnectionsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        
        [Route]
        public async Task<IHttpActionResult> GetTransferConnections([FromUri] GetTransferConnectionsQuery query)
        {
            var response = await _mediator.SendAsync(query);
            return Ok(response.TransferConnections);
        }
    }
}
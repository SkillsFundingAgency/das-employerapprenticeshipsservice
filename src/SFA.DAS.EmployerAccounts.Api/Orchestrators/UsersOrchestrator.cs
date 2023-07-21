using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
{
    public class UsersOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILogger<UsersOrchestrator> _logger;
        private readonly IMapper _mapper;

        public UsersOrchestrator(IMediator mediator, ILogger<UsersOrchestrator> logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ICollection<AccountDetail>> GetUserAccounts(string userRef)
        {
            _logger.LogInformation("Requesting user's accounts for user Ref {UserRef}", userRef);

            var accounts = await _mediator.Send(new GetUserAccountsQuery { UserRef = userRef });

            var viewModels = accounts.Accounts.AccountList.Select(x => _mapper.Map<AccountDetail>(x)).ToList();

            return viewModels;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetUserAccounts;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Api.Orchestrators
{
    public class UsersOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;

        public UsersOrchestrator(IMediator mediator, ILog logger, IMapper mapper)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<OrchestratorResponse<ICollection<AccountDetailViewModel>>> GetUserAccounts(string userRef)
        {
            _logger.Info($"Requesting user's accounts for user Ref  {userRef}");

            var accounts = await _mediator.SendAsync(new GetUserAccountsQuery {UserRef = userRef});

            var viewModels = accounts.Accounts.AccountList.Select(x => _mapper.Map<AccountDetailViewModel>(x)).ToList();

            return new OrchestratorResponse<ICollection<AccountDetailViewModel>>
            {
                Data = viewModels,
                Status = HttpStatusCode.OK
            };
        }
    }
}
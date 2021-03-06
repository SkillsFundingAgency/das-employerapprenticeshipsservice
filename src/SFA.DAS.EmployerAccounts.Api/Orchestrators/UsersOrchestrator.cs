﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Api.Types;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccounts;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
{
    public class UsersOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;

        public UsersOrchestrator(IMediator mediator, ILog logger, IMapper mapper)
        {
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<ICollection<AccountDetail>> GetUserAccounts(string userRef)
        {
            _logger.Info($"Requesting user's accounts for user Ref  {userRef}");

            var accounts = await _mediator.SendAsync(new GetUserAccountsQuery { UserRef = userRef });

            var viewModels = accounts.Accounts.AccountList.Select(x => _mapper.Map<AccountDetail>(x)).ToList();

            return viewModels;
        }
    }
}
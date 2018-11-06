using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Api.Orchestrators
{
    public class AccountsOrchestrator
    {
        private readonly IMediator _mediator;
        private readonly ILog _logger;
        private readonly IMapper _mapper;
        private readonly IHashingService _hashingService;

        public AccountsOrchestrator(IMediator mediator, ILog logger, IMapper mapper, IHashingService hashingService)
        {
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));
            _mediator = mediator;
            _logger = logger;
            _mapper = mapper;
            _hashingService = hashingService;
        }
        
        public async Task<OrchestratorResponse<ICollection<TeamMemberViewModel>>> GetAccountTeamMembers(long accountId)
        {
            var hashedAccountId = _hashingService.HashValue(accountId);

            var response = await GetAccountTeamMembers(hashedAccountId);

            return response;

        }

        public async Task<OrchestratorResponse<ICollection<TeamMemberViewModel>>> GetAccountTeamMembers(string hashedAccountId)
        {
            _logger.Info($"Requesting team members for account {hashedAccountId}");

            var teamMembers = await _mediator.SendAsync(new GetTeamMembersRequest { HashedAccountId = hashedAccountId });

            var memberViewModels = teamMembers.TeamMembers.Select(x => _mapper.Map<TeamMemberViewModel>(x)).ToList();

            return new OrchestratorResponse<ICollection<TeamMemberViewModel>>
            {
                Data = memberViewModels,
                Status = HttpStatusCode.OK
            };
        }
    }
}
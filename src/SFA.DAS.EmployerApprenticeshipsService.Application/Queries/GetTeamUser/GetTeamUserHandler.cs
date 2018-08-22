﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Queries.GetTeamUser
{
    public class GetTeamUserHandler : IAsyncRequestHandler<GetTeamMemberQuery, GetTeamMemberResponse>
    {
        private readonly IMembershipRepository _repository;
        private readonly ILog _logger;

        public GetTeamUserHandler(IMembershipRepository repository, ILog logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<GetTeamMemberResponse> Handle(GetTeamMemberQuery message)
        {
            _logger.Debug($"Getting team member for account hashed ID {message.HashedAccountId} and team member ID {message.TeamMemberId}");
            var member = await _repository.GetCaller(message.HashedAccountId, message.TeamMemberId);

            return new GetTeamMemberResponse
            {
                User = member
            };
        }
    }
}

using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetMember
{
    public class GetMemberQueryHandler : IAsyncRequestHandler<GetMemberRequest, GetMemberResponse>
    {
        private readonly IAccountTeamRepository _accountTeamRepository;
        private readonly IHashingService _hashingService;

        public GetMemberQueryHandler(IAccountTeamRepository accountTeamRepository, IHashingService hashingService)
        {
            if (accountTeamRepository == null)
                throw new ArgumentNullException(nameof(accountTeamRepository));
            _accountTeamRepository = accountTeamRepository;
            _hashingService = hashingService;
        }

        public async Task<GetMemberResponse> Handle(GetMemberRequest message)
        {
            var member = await _accountTeamRepository.GetMember(message.HashedAccountId, message.Email);

            member.HashedInvitationId = _hashingService.HashValue(member.Id);

            return new GetMemberResponse
            {
                TeamMember = member
            };
        }
    }
}
using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Queries.GetMember
{
    public class GetMemberQueryHandler : IAsyncRequestHandler<GetMemberRequest, GetMemberResponse>
    {
        private readonly IEmployerAccountTeamRepository _accountTeamRepository;
        private readonly IHashingService _hashingService;

        public GetMemberQueryHandler(IEmployerAccountTeamRepository accountTeamRepository, IHashingService hashingService)
        {
            if (accountTeamRepository == null)
                throw new ArgumentNullException(nameof(accountTeamRepository));
            _accountTeamRepository = accountTeamRepository;
            _hashingService = hashingService;
        }

        public async Task<GetMemberResponse> Handle(GetMemberRequest message)
        {
            var member = await _accountTeamRepository.GetMember(message.HashedAccountId, message.Email, message.OnlyIfMemberIsActive) ?? new Models.AccountTeam.TeamMember();
            member.HashedInvitationId = _hashingService.HashValue(member.Id);

            return new GetMemberResponse
            {
                TeamMember = member
            };
        }
    }
}
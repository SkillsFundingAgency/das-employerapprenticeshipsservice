using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Queries.GetMember;

public class GetMemberQueryHandler : IRequestHandler<GetMemberRequest, GetMemberResponse>
{
    private readonly IEmployerAccountTeamRepository _accountTeamRepository;
    private readonly IHashingService _hashingService;

    public GetMemberQueryHandler(IEmployerAccountTeamRepository accountTeamRepository, IHashingService hashingService)
    {
        _accountTeamRepository = accountTeamRepository ?? throw new ArgumentNullException(nameof(accountTeamRepository));
        _hashingService = hashingService;
    }

    public async Task<GetMemberResponse> Handle(GetMemberRequest message, CancellationToken cancellationToken)
    {
        var member = await _accountTeamRepository.GetMember(message.HashedAccountId, message.Email, message.OnlyIfMemberIsActive) ?? new TeamMember();
        member.HashedInvitationId = _hashingService.HashValue(member.Id);
        member.HashedAccountId = message.HashedAccountId;

        return new GetMemberResponse
        {
            TeamMember = member
        };
    }
}
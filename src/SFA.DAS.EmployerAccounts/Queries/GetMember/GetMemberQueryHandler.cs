using System.Threading;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.Queries.GetMember;

public class GetMemberQueryHandler : IRequestHandler<GetMemberRequest, GetMemberResponse>
{
    private readonly IEmployerAccountTeamRepository _accountTeamRepository;
    private readonly IEncodingService _encodingService;

    public GetMemberQueryHandler(IEmployerAccountTeamRepository accountTeamRepository, IEncodingService encodingService)
    {
        _accountTeamRepository = accountTeamRepository ?? throw new ArgumentNullException(nameof(accountTeamRepository));
        _encodingService = encodingService;
    }

    public async Task<GetMemberResponse> Handle(GetMemberRequest message, CancellationToken cancellationToken)
    {
        var hashedAccountId = _encodingService.Encode(message.AccountId, EncodingType.AccountId);
        var member = await _accountTeamRepository.GetMember(hashedAccountId, message.Email, message.OnlyIfMemberIsActive) ?? new TeamMember();
        member.HashedInvitationId = _encodingService.Encode(member.Id, EncodingType.AccountId);
        member.HashedAccountId = hashedAccountId;

        return new GetMemberResponse
        {
            TeamMember = member
        };
    }
}
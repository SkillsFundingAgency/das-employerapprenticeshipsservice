using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetMember
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
            var member = await _accountTeamRepository.GetMember(message.HashedId, message.Email);

            member.HashedInvitationId = _hashingService.HashValue(member.Id);

            return new GetMemberResponse
            {
                TeamMember = member
            };
        }
    }
}
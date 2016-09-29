using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetMember
{
    public class GetMemberQueryHandler : IAsyncRequestHandler<GetMemberRequest, GetMemberResponse>
    {
        private readonly IAccountTeamRepository _accountTeamRepository;

        public GetMemberQueryHandler(IAccountTeamRepository accountTeamRepository)
        {
            if (accountTeamRepository == null)
                throw new ArgumentNullException(nameof(accountTeamRepository));
            _accountTeamRepository = accountTeamRepository;
        }

        public async Task<GetMemberResponse> Handle(GetMemberRequest message)
        {
            var member = await _accountTeamRepository.GetMember(message.HashedId, message.Email);

            return new GetMemberResponse
            {
                TeamMember = member
            };
        }
    }
}
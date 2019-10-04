using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountOwner
{
    public class GetAccountOwnerHandler : IAsyncRequestHandler<GetAccountOwnerQuery, GetAccountOwnerResponse>
    {
        private readonly IEmployerAccountTeamRepository _repository;

        public GetAccountOwnerHandler(IEmployerAccountTeamRepository repository)
        {
            _repository = repository;
        }

        public async Task<GetAccountOwnerResponse> Handle(GetAccountOwnerQuery message)
        {
            var accounts = await _repository.GetAccountTeamMembers(message.HashedAccountId);
            return new GetAccountOwnerResponse { TeamMembers = accounts.First(tm => tm.Role == DAS.Authorization.Role.Owner) };
        }
    }
}

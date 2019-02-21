using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries
{
    internal class IsUserInAnyRoleQueryHandler : IReadStoreRequestHandler<IsUserInAnyRoleQuery, bool>
    {
        private readonly IAccountUsersRepository _accountUsersRepository;

        public IsUserInAnyRoleQueryHandler(IAccountUsersRepository accountUsersRepository)
        {
            _accountUsersRepository = accountUsersRepository;
        }

        public Task<bool> Handle(IsUserInAnyRoleQuery request, CancellationToken cancellationToken)
        {
            return _accountUsersRepository
                .CreateQuery()
                .AnyAsync(r => 
                    r.UserRef == request.UserRef && 
                    r.AccountId == request.AccountId && 
                    r.Removed == null, cancellationToken);
        }
    }
}
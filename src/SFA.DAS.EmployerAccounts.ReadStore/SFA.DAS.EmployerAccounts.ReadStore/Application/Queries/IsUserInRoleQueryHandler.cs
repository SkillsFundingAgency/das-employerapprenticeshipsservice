using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Queries
{
    internal class IsUserInRoleQueryHandler : IReadStoreRequestHandler<IsUserInRoleQuery, bool>
    {
        private readonly IAccountUsersRepository _accountUsersRepository;

        public IsUserInRoleQueryHandler(IAccountUsersRepository accountUsersRepository)
        {
            _accountUsersRepository = accountUsersRepository;
        }

        public Task<bool> Handle(IsUserInRoleQuery request, CancellationToken cancellationToken)
        {
            return _accountUsersRepository
                .CreateQuery()
                .AnyAsync(r => 
                    r.UserRef == request.UserRef && 
                    r.AccountId == request.AccountId && 
                    r.Removed == null &&
                    r.Role != null && request.UserRoles.Contains(r.Role.Value), cancellationToken);
        }
    }
}
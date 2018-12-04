using System.Threading;
using System.Threading.Tasks;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    internal class UpdateAccountUserCommandHandler : IReadStoreRequestHandler<UpdateAccountUserCommand, Unit>
    {
        private readonly IAccountUsersRepository _accountUsersRepository;

        public UpdateAccountUserCommandHandler(IAccountUsersRepository accountUsersRepository)
        {
            _accountUsersRepository = accountUsersRepository;
        }
        public async Task<Unit> Handle(UpdateAccountUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _accountUsersRepository.CreateQuery().SingleAsync(x => x.UserRef == request.UserRef && x.AccountId == request.AccountId, cancellationToken);

            user.UpdateRoles(request.Roles, request.Updated, request.MessageId);
            await _accountUsersRepository.Update(user, null, cancellationToken);

            return Unit.Value;
        }
    }
}

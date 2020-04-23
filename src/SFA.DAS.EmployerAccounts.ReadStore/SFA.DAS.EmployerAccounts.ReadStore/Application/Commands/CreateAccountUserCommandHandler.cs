using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using SFA.DAS.CosmosDb;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Exceptions;
using SFA.DAS.EmployerAccounts.ReadStore.Mediator;
using SFA.DAS.EmployerAccounts.ReadStore.Models;

namespace SFA.DAS.EmployerAccounts.ReadStore.Application.Commands
{
    internal class CreateAccountUserCommandHandler : IReadStoreRequestHandler<CreateAccountUserCommand, Unit>
    {
        private readonly IAccountUsersRepository _accountUsersRepository;
        private readonly ILogger _logger;

        public CreateAccountUserCommandHandler(IAccountUsersRepository accountUsersRepository, ILogger logger)
        {
            _accountUsersRepository = accountUsersRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(CreateAccountUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await _accountUsersRepository.CreateQuery().SingleOrDefaultAsync(x => x.UserRef == request.UserRef && x.AccountId == request.AccountId, cancellationToken);

                if (user == null)
                {
                    user = new AccountUser(request.UserRef, request.AccountId, request.Role, request.Created, request.MessageId);
                    await _accountUsersRepository.Add(user, null, cancellationToken);
                }
                else
                {
                    user.Recreate(request.Role, request.Created, request.MessageId);
                    await _accountUsersRepository.Update(user, null, cancellationToken);
                }
            }
            catch (UserNotRemovedException exception)
            {
                _logger.LogWarning("User has not been removed", exception);
            }
            return Unit.Value;
        }
    }
}

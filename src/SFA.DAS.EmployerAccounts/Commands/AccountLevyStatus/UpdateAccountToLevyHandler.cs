using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;

namespace SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus
{
    public class UpdateAccountToLevyHandler : AsyncRequestHandler<UpdateAccountToLevy>
    {
        private readonly IEmployerAccountRepository _accountRepositoryObject;

        public UpdateAccountToLevyHandler(IEmployerAccountRepository accountRepositoryObject)
        {
            _accountRepositoryObject = accountRepositoryObject;
        }

        protected override Task HandleCore(UpdateAccountToLevy message)
        {
            return
            _accountRepositoryObject
                .SetAccountAsLevy(
                    message
                        .AccountId);
        }
    }
}
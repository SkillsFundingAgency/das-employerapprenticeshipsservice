using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerAccounts.Commands.AccountLevyStatus
{
    public class UpdateAccountToLevyHandler : AsyncRequestHandler<UpdateAccountToLevy>
    {
        private readonly IEmployerAccountRepository _accountRepositoryObject;
        private readonly ILog _logger;

        public UpdateAccountToLevyHandler(
            IEmployerAccountRepository accountRepositoryObject,
            ILog logger)
        {
            _accountRepositoryObject = accountRepositoryObject;
            _logger = logger;
        }

        protected override Task HandleCore(UpdateAccountToLevy command)
        {
            _logger
                .Info(
                    UpdatedStartedMessage(
                        command));

            return
                _accountRepositoryObject
                    .SetAccountAsLevy(
                        command
                        .AccountId)
                    .ContinueWith(
                        t =>
                            _logger
                            .Info(
                                    UpdateCompleteMessage(command)),
                        TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        public string UpdatedStartedMessage(UpdateAccountToLevy updateCommand)
        {
            return
                $"About to update Account with id: {updateCommand.AccountId} to Levy status.";
        }

        public string UpdateCompleteMessage(UpdateAccountToLevy updateCommand)
        {
            return
                $"Updated Account with id: {updateCommand.AccountId} to Levy status.";
        }
    }
}
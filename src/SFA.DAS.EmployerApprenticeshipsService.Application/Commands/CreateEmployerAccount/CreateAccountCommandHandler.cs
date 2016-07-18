using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccount
{
    public class CreateAccountCommandHandler : AsyncRequestHandler<CreateAccountCommand>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;

        public CreateAccountCommandHandler(IAccountRepository accountRepository, IMessagePublisher messagePublisher)
        {
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            if (messagePublisher == null)
                throw new ArgumentNullException(nameof(messagePublisher));
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(CreateAccountCommand message)
        {
            //TODO: Validate

            var accountId = await _accountRepository.CreateAccount(message.UserId, message.CompanyNumber, message.CompanyName, message.EmployerRef);

            await _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage
            {
                AccountId = accountId
            });
        }
    }
}
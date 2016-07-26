using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Attributes;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccount
{
    public class CreateAccountCommandHandler : AsyncRequestHandler<CreateAccountCommand>
    {
        [QueueName]
        public string das_at_eas_get_employer_levy { get; set; }

        private readonly IAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly CreateAccountCommandValidator _validator;

        public CreateAccountCommandHandler(IAccountRepository accountRepository, IMessagePublisher messagePublisher)
        {
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            if (messagePublisher == null)
                throw new ArgumentNullException(nameof(messagePublisher));
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
            _validator = new CreateAccountCommandValidator();
        }

        protected override async Task HandleCore(CreateAccountCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var accountId = await _accountRepository.CreateAccount(message.UserId, message.CompanyNumber, message.CompanyName, message.EmployerRef);

            await _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage
            {
                AccountId = accountId
            });
        }
    }
}
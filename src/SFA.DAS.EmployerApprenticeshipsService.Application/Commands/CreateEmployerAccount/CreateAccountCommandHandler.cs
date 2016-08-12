using System;
using System.Collections.Generic;
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
        public string get_employer_levy { get; set; }

        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly CreateAccountCommandValidator _validator;

        public CreateAccountCommandHandler(IAccountRepository accountRepository, IUserRepository userRepository, IMessagePublisher messagePublisher)
        {
            if (accountRepository == null)
                throw new ArgumentNullException(nameof(accountRepository));
            if (userRepository == null)
                throw new ArgumentNullException(nameof(userRepository));
            if (messagePublisher == null)
                throw new ArgumentNullException(nameof(messagePublisher));
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _messagePublisher = messagePublisher;
            _validator = new CreateAccountCommandValidator();
        }

        protected override async Task HandleCore(CreateAccountCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var user = await _userRepository.GetById(message.ExternalUserId);

            if (user == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "User", "User does not exist" } });

            var accountId = await _accountRepository.CreateAccount(user.Id, message.CompanyNumber, message.CompanyName, message.CompanyRegisteredAddress, message.CompanyDateOfIncorporation, message.EmployerRef, message.AccessToken, message.RefreshToken);

            await _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage
            {
                AccountId = accountId
            });
        }
    }
}
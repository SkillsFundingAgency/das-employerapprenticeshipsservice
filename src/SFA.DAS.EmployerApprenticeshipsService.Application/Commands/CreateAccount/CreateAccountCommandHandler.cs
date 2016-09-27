using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAccountCreatedNotification;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Attributes;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateAccount
{
    public class CreateAccountCommandHandler : IAsyncRequestHandler<CreateAccountCommand, CreateAccountCommandResponse>
    {
        [QueueName]
        public string get_employer_levy { get; set; }

        private readonly IAccountRepository _accountRepository;
        private readonly IUserRepository _userRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IMediator _mediator;
        private readonly IValidator<CreateAccountCommand> _validator;

        public CreateAccountCommandHandler(IAccountRepository accountRepository, IUserRepository userRepository, IMessagePublisher messagePublisher, IMediator mediator, IValidator<CreateAccountCommand> validator)
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
            _mediator = mediator;
            _validator = validator;
        }

        public async Task<CreateAccountCommandResponse> Handle(CreateAccountCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var user = await _userRepository.GetById(message.ExternalUserId);

            if (user == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "User", "User does not exist" } });

            var emprefs = message.EmployerRef.Split(',');

            var accountId = await _accountRepository.CreateAccount(user.Id, message.CompanyNumber, message.CompanyName, message.CompanyRegisteredAddress, message.CompanyDateOfIncorporation, emprefs[0], message.AccessToken, message.RefreshToken);

            if (emprefs.Length > 1)
            {
                var schemes = await _accountRepository.GetPayeSchemes(accountId);
                for (var i = 1; i < emprefs.Length; i++)
                {
                    await _accountRepository.AddPayeToAccountForExistingLegalEntity(accountId, schemes.First().LegalEntityId, emprefs[i], message.AccessToken, message.RefreshToken);
                }
            }

            await _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage
            {
                AccountId = accountId
            });

            await _mediator.SendAsync(new CreateEmployerAccountCreatedNotificationCommand
            {
                AccountId = accountId
            });

            return new CreateAccountCommandResponse
            {
                AccountId = accountId
            };
        }
    }
}
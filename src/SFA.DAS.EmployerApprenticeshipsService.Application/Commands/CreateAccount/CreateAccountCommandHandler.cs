using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.Application.Commands.CreateAccount
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
        private readonly IHashingService _hashingService;

        public CreateAccountCommandHandler(IAccountRepository accountRepository, IUserRepository userRepository, IMessagePublisher messagePublisher, IMediator mediator, IValidator<CreateAccountCommand> validator, IHashingService hashingService)
        {
            _accountRepository = accountRepository;
            _userRepository = userRepository;
            _messagePublisher = messagePublisher;
            _mediator = mediator;
            _validator = validator;
            _hashingService = hashingService;
        }

        public async Task<CreateAccountCommandResponse> Handle(CreateAccountCommand message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var user = await _userRepository.GetByUserRef(message.ExternalUserId);

            if (user == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "User", "User does not exist" } });

            var emprefs = message.EmployerRef.Split(',');

            var accountId = await _accountRepository.CreateAccount(user.Id, message.CompanyNumber, message.CompanyName, message.CompanyRegisteredAddress, message.CompanyDateOfIncorporation, emprefs[0], message.AccessToken, message.RefreshToken);

            var hashedAccountId = _hashingService.HashValue(accountId);
            await _accountRepository.SetHashedId(hashedAccountId, accountId);

            if (emprefs.Length > 1)
            {
                for (var i = 1; i < emprefs.Length; i++)
                {
                    await _accountRepository.AddPayeToAccount(new Paye {AccountId= accountId,EmpRef= emprefs[i], AccessToken= message.AccessToken, RefreshToken = message.RefreshToken});
                }
            }
            

            await _messagePublisher.PublishAsync(new EmployerRefreshLevyQueueMessage
            {
                AccountId = accountId
            });
            
            return new CreateAccountCommandResponse
            {
                HashedAccountId = hashedAccountId
            };
        }
    }
}
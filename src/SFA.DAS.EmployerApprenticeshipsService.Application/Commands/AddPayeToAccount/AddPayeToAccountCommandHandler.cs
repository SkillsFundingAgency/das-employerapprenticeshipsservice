using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.Application.Commands.AddPayeToAccount
{
    public class AddPayeToAccountCommandHandler : AsyncRequestHandler<AddPayeToAccountCommand>
    {
        [QueueName]
        public string get_employer_levy { get; set; }

        private readonly IValidator<AddPayeToAccountCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IHashingService _hashingService;

        public AddPayeToAccountCommandHandler(IValidator<AddPayeToAccountCommand> validator, IAccountRepository accountRepository, IMessagePublisher messagePublisher, IHashingService hashingService)
        {
            _validator = validator;
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
            _hashingService = hashingService;
        }

        protected override async Task HandleCore(AddPayeToAccountCommand message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var accountId = _hashingService.DecodeValue(message.HashedAccountId);

            await _accountRepository.AddPayeToAccount(
                    new Paye
                    {
                        AccessToken = message.AccessToken,
                        RefreshToken = message.RefreshToken,
                        AccountId = accountId,
                        EmpRef = message.Empref,
                        RefName = message.EmprefName
                    }
                );

            await _messagePublisher.PublishAsync(
                new EmployerRefreshLevyQueueMessage
                {
                    AccountId = accountId
                });
        }
    }
}
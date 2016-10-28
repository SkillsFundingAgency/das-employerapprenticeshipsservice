using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Attributes;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.Application.Commands.AddPayeToNewLegalEntity
{
    public class AddPayeToNewLegalEnttiyCommandHandler : AsyncRequestHandler<AddPayeToNewLegalEntityCommand>
    {
        [QueueName]
        public string get_employer_levy { get; set; }

        private readonly IValidator<AddPayeToNewLegalEntityCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IHashingService _hashingService;

        public AddPayeToNewLegalEnttiyCommandHandler(IValidator<AddPayeToNewLegalEntityCommand> validator, IAccountRepository accountRepository, IMessagePublisher messagePublisher, IHashingService hashingService)
        {
            _validator = validator;
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
            _hashingService = hashingService;
        }

        protected override async Task HandleCore(AddPayeToNewLegalEntityCommand message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            var accountId = _hashingService.DecodeValue(message.HashedId);

            await _accountRepository.AddPayeToAccountForNewLegalEntity(
                    new Paye
                    {
                        AccessToken = message.AccessToken,
                        RefreshToken = message.RefreshToken,
                        AccountId = accountId,
                        EmpRef = message.Empref
                    }, 
                    new LegalEntity
                    {
                        Name = message.LegalEntityName,
                        Code = message.LegalEntityCode,
                        DateOfIncorporation = message.LegalEntityDate,
                        RegisteredAddress = message.LegalEntityAddress
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
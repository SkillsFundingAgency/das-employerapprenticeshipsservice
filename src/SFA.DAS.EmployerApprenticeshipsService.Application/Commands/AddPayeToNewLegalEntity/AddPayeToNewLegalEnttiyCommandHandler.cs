using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Application.Validation;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Attributes;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Entities.Account;
using SFA.DAS.Messaging;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.AddPayeToNewLegalEntity
{
    public class AddPayeToNewLegalEnttiyCommandHandler : AsyncRequestHandler<AddPayeToNewLegalEntityCommand>
    {
        [QueueName]
        public string get_employer_levy { get; set; }

        private readonly IValidator<AddPayeToNewLegalEntityCommand> _validator;
        private readonly IAccountRepository _accountRepository;
        private readonly IMessagePublisher _messagePublisher;

        public AddPayeToNewLegalEnttiyCommandHandler(IValidator<AddPayeToNewLegalEntityCommand> validator, IAccountRepository accountRepository, IMessagePublisher messagePublisher)
        {
            _validator = validator;
            _accountRepository = accountRepository;
            _messagePublisher = messagePublisher;
        }

        protected override async Task HandleCore(AddPayeToNewLegalEntityCommand message)
        {
            var result = await _validator.ValidateAsync(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            await _accountRepository.AddPayeToAccountForNewLegalEntity(
                    new Paye
                    {
                        AccessToken = message.AccessToken,
                        RefreshToken = message.RefreshToken,
                        AccountId = message.AccountId,
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
                    AccountId = message.AccountId
                });
        }
    }
}
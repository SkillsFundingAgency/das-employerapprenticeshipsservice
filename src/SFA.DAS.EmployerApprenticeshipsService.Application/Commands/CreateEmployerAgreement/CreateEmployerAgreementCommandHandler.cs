using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EmployerAccounts.Events.Messages;
using SFA.DAS.Messaging.Interfaces;

namespace SFA.DAS.EAS.Application.Commands.CreateEmployerAgreement
{
    public class CreateEmployerAgreementCommandHandler : AsyncRequestHandler<CreateEmployerAgreementCommand>
    {
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IMessagePublisher _messagePublisher;
        private readonly IValidator<CreateEmployerAgreementCommand> _validator;

        public CreateEmployerAgreementCommandHandler(
            IEmployerAgreementRepository employerAgreementRepository,
            IMessagePublisher messagePublisher,
            IValidator<CreateEmployerAgreementCommand> validator
            )
        {
            _employerAgreementRepository = employerAgreementRepository ?? throw new ArgumentNullException(nameof(employerAgreementRepository));
            _messagePublisher = messagePublisher ?? throw new ArgumentNullException(nameof(messagePublisher));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        protected override async Task HandleCore(CreateEmployerAgreementCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var newAgreementId = await _employerAgreementRepository.CreateEmployerAgreeement(
                                message.LatestTemplateId, 
                                message.AccountId, 
                                message.LegalEntityId);

            var publishMessage = new AgreementCreatedMessage(
                message.AccountId, 
                newAgreementId, 
                null,
                message.LegalEntityId, 
                null, 
                null);

            await _messagePublisher.PublishAsync(publishMessage);
        }
    }
}
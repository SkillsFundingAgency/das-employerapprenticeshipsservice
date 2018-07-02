using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NServiceBus;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Messages.Events;

namespace SFA.DAS.EAS.Application.Commands.CreateEmployerAgreement
{
    public class CreateEmployerAgreementCommandHandler : AsyncRequestHandler<CreateEmployerAgreementCommand>
    {
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IEventPublisher _eventPublisher;
        private readonly IValidator<CreateEmployerAgreementCommand> _validator;

        public CreateEmployerAgreementCommandHandler(
            IEmployerAgreementRepository employerAgreementRepository,
            IEventPublisher eventPublisher,
            IValidator<CreateEmployerAgreementCommand> validator)
        {
            _employerAgreementRepository = employerAgreementRepository;
            _eventPublisher = eventPublisher;
            _validator = validator;
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

            await _eventPublisher.Publish<CreatedAgreementEvent>(c =>
            {
                c.AccountId = message.AccountId;
                c.AgreementId = newAgreementId;
                c.LegalEntityId = message.LegalEntityId;
            });
        }
    }
}
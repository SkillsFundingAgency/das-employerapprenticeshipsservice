using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Commands.SetAccountLegalEntityAgreementStatus
{
    public class SetAccountLegalEntityAgreementStatusCommandHandler : AsyncRequestHandler<SetAccountLegalEntityAgreementStatusCommand>
    {
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IValidator<SetAccountLegalEntityAgreementStatusCommand> _validator;

        public SetAccountLegalEntityAgreementStatusCommandHandler(
            IEmployerAgreementRepository employerAgreementRepository,
            IValidator<SetAccountLegalEntityAgreementStatusCommand> validator
            )
        {
            _employerAgreementRepository = employerAgreementRepository ?? throw new ArgumentNullException(nameof(employerAgreementRepository));
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        protected override async Task HandleCore(SetAccountLegalEntityAgreementStatusCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            await _employerAgreementRepository.EvaluateEmployerLegalEntityAgreementStatus(
                                message.AccountId, 
                                message.LegalEntityId);
        }
    }
}
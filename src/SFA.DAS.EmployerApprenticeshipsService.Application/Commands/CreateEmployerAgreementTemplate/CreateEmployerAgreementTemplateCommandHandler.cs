using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateEmployerAgreementTemplate
{
    public class CreateEmployerAgreementTemplateCommandHandler : AsyncRequestHandler<CreateEmployerAgreementTemplateCommand>
    {
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly CreateEmployerAgreementTemplateCommandValidator _validator;

        public CreateEmployerAgreementTemplateCommandHandler(IEmployerAgreementRepository employerAgreementRepository)
        {
            if (employerAgreementRepository == null)
                throw new ArgumentNullException(nameof(employerAgreementRepository));
            _employerAgreementRepository = employerAgreementRepository;
            _validator = new CreateEmployerAgreementTemplateCommandValidator();
        }

        protected override async Task HandleCore(CreateEmployerAgreementTemplateCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            await _employerAgreementRepository.CreateEmployerAgreementTemplate(message.Text);
        }
    }
}
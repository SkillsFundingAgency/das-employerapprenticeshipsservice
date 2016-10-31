using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Domain.Data;

namespace SFA.DAS.EAS.Application.Commands.ReleaseEmployerAgreementTemplate
{
    public class ReleaseEmployerAgreementTemplateCommandHandler : AsyncRequestHandler<ReleaseEmployerAgreementTemplateCommand>
    {
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly ReleaseEmployerAgreementTemplateCommandValidator _validator;

        public ReleaseEmployerAgreementTemplateCommandHandler(IEmployerAgreementRepository employerAgreementRepository)
        {
            if (employerAgreementRepository == null)
                throw new ArgumentNullException(nameof(employerAgreementRepository));
            _employerAgreementRepository = employerAgreementRepository;
            _validator = new ReleaseEmployerAgreementTemplateCommandValidator();
        }

        protected override async Task HandleCore(ReleaseEmployerAgreementTemplateCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var template = await _employerAgreementRepository.GetEmployerAgreementTemplate(message.TemplateId);

            if (template == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "AgreementTemplate", $"AgreementTemplate {message.TemplateId} does not exist" } });
            if (template.ReleasedDate.HasValue)
                throw new InvalidRequestException(new Dictionary<string, string> { { "AgreementTemplate", $"AgreementTemplate {message.TemplateId} has already been released" } });

            await _employerAgreementRepository.ReleaseEmployerAgreementTemplate(message.TemplateId);
        }
    }
}
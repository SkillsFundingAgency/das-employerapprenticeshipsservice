﻿using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Validation;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.Hmrc;

namespace SFA.DAS.EmployerFinance.Commands.UpdatePayeInformation
{
    public class UpdatePayeInformationCommandHandler : AsyncRequestHandler<UpdatePayeInformationCommand>
    {
        private readonly IValidator<UpdatePayeInformationCommand> _validator;
        private readonly IPayeRepository _payeRepository;
        private readonly IHmrcService _hmrcService;

        public UpdatePayeInformationCommandHandler(IValidator<UpdatePayeInformationCommand> validator, IPayeRepository payeRepository, IHmrcService hmrcService)
        {
            _validator = validator;
            _payeRepository = payeRepository;
            _hmrcService = hmrcService;
        }

        protected override async Task HandleCore(UpdatePayeInformationCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var scheme = await _payeRepository.GetPayeSchemeByRef(message.PayeRef);

            if (!string.IsNullOrEmpty(scheme?.RefName))
            {
                return;
            }

            var result = await _hmrcService.GetEmprefInformation(scheme?.Ref);

            if (string.IsNullOrEmpty(result?.Employer?.Name?.EmprefAssociatedName))
            {
                return;
            }

            await _payeRepository.UpdatePayeSchemeName(message.PayeRef, result.Employer.Name.EmprefAssociatedName);
        }
    }
}
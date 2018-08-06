﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.Queries.GetEmployerAgreementById
{
    public class GetEmployerAgreementByIdRequestHandler : IAsyncRequestHandler<GetEmployerAgreementByIdRequest, GetEmployerAgreementByIdResponse>
    {
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetEmployerAgreementByIdRequest> _validator;

        public GetEmployerAgreementByIdRequestHandler(
            IEmployerAgreementRepository employerAgreementRepository, 
            IHashingService hashingService, 
            IValidator<GetEmployerAgreementByIdRequest> validator)
        {
            _employerAgreementRepository = employerAgreementRepository;
            _hashingService = hashingService;
            _validator = validator;
        }

        public async Task<GetEmployerAgreementByIdResponse> Handle(GetEmployerAgreementByIdRequest message)
        {
            var validationResult = _validator.Validate(message);
            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            var agreementId = _hashingService.DecodeValue(message.HashedAgreementId);

            var agreement = await _employerAgreementRepository.GetEmployerAgreement(agreementId);

            if (agreement == null)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Agreement", "The agreement could not be found" } });

            agreement.HashedAgreementId = message.HashedAgreementId;
            return new GetEmployerAgreementByIdResponse
            {
                EmployerAgreement = agreement
            };
        }
    }
}
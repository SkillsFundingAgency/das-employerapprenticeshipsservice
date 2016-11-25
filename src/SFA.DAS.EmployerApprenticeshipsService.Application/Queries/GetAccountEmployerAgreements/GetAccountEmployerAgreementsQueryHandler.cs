using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    //TODO tests need adding and validator
    public class GetAccountEmployerAgreementsQueryHandler : IAsyncRequestHandler<GetAccountEmployerAgreementsRequest, GetAccountEmployerAgreementsResponse>
    {
        private readonly IAccountRepository _accountRepository;
        private readonly IHashingService _hashingService;
        private readonly IValidator<GetAccountEmployerAgreementsRequest> _validator;

        public GetAccountEmployerAgreementsQueryHandler(IAccountRepository accountRepository, IHashingService hashingService, IValidator<GetAccountEmployerAgreementsRequest> validator)
        {
            _accountRepository = accountRepository;
            _hashingService = hashingService;
            _validator = validator;
        }

        public async Task<GetAccountEmployerAgreementsResponse> Handle(GetAccountEmployerAgreementsRequest message)
        {
            var validationResult = await _validator.ValidateAsync(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }
            if (validationResult.IsUnauthorized)
            {
                throw new UnauthorizedAccessException();
            }

            var agreements = await _accountRepository.GetEmployerAgreementsLinkedToAccount(_hashingService.DecodeValue(message.HashedId));

            foreach (var agreement in agreements)
            {
                agreement.HashedAgreementId = _hashingService.HashValue(agreement.Id);
            }

            return new GetAccountEmployerAgreementsResponse
            {
                EmployerAgreements = agreements
            };
        }
    }
}
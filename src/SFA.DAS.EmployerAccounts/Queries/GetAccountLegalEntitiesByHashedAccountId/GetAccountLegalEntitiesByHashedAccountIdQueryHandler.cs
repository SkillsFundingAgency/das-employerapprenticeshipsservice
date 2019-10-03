using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntitiesByHashedAccountId
{
    public class GetAccountLegalEntitiesByHashedAccountIdQueryHandler : IAsyncRequestHandler<GetAccountLegalEntitiesByHashedAccountIdRequest, GetAccountLegalEntitiesByHashedAccountIdResponse>
    {
        private readonly IHashingService _hashingService;
        private readonly IEmployerAgreementRepository _employerAgreementRepository;
        private readonly IValidator<GetAccountLegalEntitiesByHashedAccountIdRequest> _validator;

        public GetAccountLegalEntitiesByHashedAccountIdQueryHandler(
            IHashingService hashingService,
            IEmployerAgreementRepository employerAgreementRepository,
            IValidator<GetAccountLegalEntitiesByHashedAccountIdRequest> validator)
        {
            _hashingService = hashingService;
            _employerAgreementRepository = employerAgreementRepository;
            _validator = validator;
        }

        public async Task<GetAccountLegalEntitiesByHashedAccountIdResponse> Handle(GetAccountLegalEntitiesByHashedAccountIdRequest message)
        {
            var result = _validator.Validate(message);

            if (!result.IsValid())
            {
                throw new InvalidRequestException(result.ValidationDictionary);
            }

            long accountId;

            if (
                _hashingService.TryDecodeValue(
                    message.HashedAccountId,
                    out accountId))
            {
                var accountSpecificLegalEntity = await _employerAgreementRepository.GetLegalEntitiesLinkedToAccount(
                    accountId,
                    false);

                return new GetAccountLegalEntitiesByHashedAccountIdResponse
                {
                    LegalEntities = accountSpecificLegalEntity.ToList()
                };
            }

            throw new InvalidRequestException(
                new Dictionary<string, string>
                {
                    {
                        nameof(message.HashedAccountId), "Hashed account ID cannot be decoded."
                    }
                }
            );
        }
    }
}

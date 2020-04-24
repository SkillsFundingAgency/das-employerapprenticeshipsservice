﻿using MediatR;
using System.Threading.Tasks;
using SFA.DAS.Validation;
using SFA.DAS.EmployerAccounts.Interfaces;
using System.Linq;
using SFA.DAS.HashingService;

namespace SFA.DAS.EmployerAccounts.Queries.GetSingleCohort
{
    public class GetSingleCohortRequestHandler : IAsyncRequestHandler<GetSingleCohortRequest, GetSingleCohortResponse>
    {
        private readonly IValidator<GetSingleCohortRequest> _validator;
        private readonly ICommitmentV2Service _commitmentV2Service;        
        private readonly IHashingService _hashingService;

        public GetSingleCohortRequestHandler(
            IValidator<GetSingleCohortRequest> validator,
            ICommitmentV2Service commitmentV2Service,            
            IHashingService hashingService)
        {
            _validator = validator;
            _commitmentV2Service = commitmentV2Service;            
            _hashingService = hashingService;
        }

        public async Task<GetSingleCohortResponse> Handle(GetSingleCohortRequest message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            long accountId = _hashingService.DecodeValue(message.HashedAccountId);            

            var cohortsResponse = await _commitmentV2Service.GetCohorts(accountId);
            if (cohortsResponse.Count() != 1) return new GetSingleCohortResponse();

            var singleCohort = cohortsResponse.Single();

            if (singleCohort.NumberOfDraftApprentices > 0)
            {
                singleCohort.Apprenticeships = await _commitmentV2Service.GetDraftApprenticeships(singleCohort);                
            }

            return new GetSingleCohortResponse
            {
                Cohort = singleCohort                
            };
        }
    }
}

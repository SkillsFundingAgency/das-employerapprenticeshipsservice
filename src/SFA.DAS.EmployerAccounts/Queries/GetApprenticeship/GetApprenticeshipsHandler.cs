using System;
using MediatR;
using System.Threading.Tasks;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.HashingService;
using System;

namespace SFA.DAS.EmployerAccounts.Queries.GetApprenticeship
{
    public class GetApprenticeshipsHandler : IAsyncRequestHandler<GetApprenticeshipsRequest, GetApprenticeshipsResponse>
    {
        private readonly IValidator<GetApprenticeshipsRequest> _validator;
        private readonly ILog _logger;        
        private readonly ICommitmentV2Service _commitmentV2Service;
        private readonly IHashingService _hashingService;
        private readonly EmployerAccountsConfiguration _employerAccountsConfiguration;
        public GetApprenticeshipsHandler(
            IValidator<GetApprenticeshipsRequest> validator,
            ILog logger,
            ICommitmentV2Service commitmentV2Service,
            IHashingService hashingService, EmployerAccountsConfiguration employerAccountsConfiguration)
        {
            _validator = validator;
            _logger = logger;            
            _commitmentV2Service = commitmentV2Service;
            _hashingService = hashingService;
            _employerAccountsConfiguration = employerAccountsConfiguration;
        }
        
        public async Task<GetApprenticeshipsResponse> Handle(GetApprenticeshipsRequest message)
        {
            var validationResult = _validator.Validate(message);

            if(!validationResult.IsValid())
            {
                throw new InvalidRequestException(validationResult.ValidationDictionary);
            }

            long accountId = _hashingService.DecodeValue(message.HashedAccountId);

            try
            {
                return new GetApprenticeshipsResponse
                {
                    Apprenticeships = await _commitmentV2Service.GetApprenticeships(accountId)
                };
            }
            catch(Exception ex)
            {
                _logger.Error(ex, $"Failed to get Cohorts for {message.HashedAccountId}");
                return new GetApprenticeshipsResponse
                {
                    HasFailed = true
                };
            }
        }
    }
}

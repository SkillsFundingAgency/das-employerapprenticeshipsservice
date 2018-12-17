using System;
using System.Threading.Tasks;
using SFA.DAS.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements
{
    public class GetAccountEmployerAgreementsValidator : IValidator<GetAccountEmployerAgreementsRequest> 
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly ILog _logger;

        public GetAccountEmployerAgreementsValidator(IMembershipRepository membershipRepository, ILog logger)
        {
            _membershipRepository = membershipRepository;
            _logger = logger;
        }

        public ValidationResult Validate(GetAccountEmployerAgreementsRequest item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetAccountEmployerAgreementsRequest item)
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                validationResult.AddError(nameof(item.ExternalUserId),"ExternalUserId has not been supplied");
            }
            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId), "HashedAccountId has not been supplied");
            }

            if (!validationResult.IsValid())
            {
                return validationResult;
            }

            var membership = await _membershipRepository.GetCaller(item.HashedAccountId, item.ExternalUserId);

            if (membership == null)
            {
                _logger.Warn($"Unauthorised user='{item.ExternalUserId}' account='{item.HashedAccountId}'");

                validationResult.IsUnauthorized = true;
            }
                

            return validationResult;
        }
    }
}

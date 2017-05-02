using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;


namespace SFA.DAS.EAS.Application.Queries.AccountTransactions.GetPreviousTransactionsCount
{
    public class GetPreviousTransactionsCountRequestValidator : IValidator<GetPreviousTransactionsCountRequest>
    {
        private readonly IMembershipRepository _membershipRepository;

        public GetPreviousTransactionsCountRequestValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(GetPreviousTransactionsCountRequest request)
        {
           throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(GetPreviousTransactionsCountRequest request)
        {
            var validationResult = new ValidationResult();

            if (request.AccountId <= 0)
            {
                validationResult.AddError(nameof(request.AccountId));
            }

            if (request.FromDate.Equals(default(DateTime)))
            {
                validationResult.AddError(nameof(request.FromDate));
            }

            if (validationResult.IsValid())
            {
                var member = await _membershipRepository.GetCaller(request.AccountId, request.ExternalUserId);
                if (member == null)
                {
                    validationResult.AddError(nameof(member), "Unauthorised: User not connected to account");
                    validationResult.IsUnauthorized = true;
                }
            }

            return validationResult;
        }
    }
}

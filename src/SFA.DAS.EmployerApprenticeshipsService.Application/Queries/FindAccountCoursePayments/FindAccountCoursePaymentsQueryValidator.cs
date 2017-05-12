using System;
using System.Threading.Tasks;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EAS.Application.Queries.FindAccountCoursePayments
{
    public class FindAccountCoursePaymentsQueryValidator : IValidator<FindAccountCoursePaymentsQuery>
    {
        private readonly IMembershipRepository _membershipRepository;

        public FindAccountCoursePaymentsQueryValidator(IMembershipRepository membershipRepository)
        {
            _membershipRepository = membershipRepository;
        }

        public ValidationResult Validate(FindAccountCoursePaymentsQuery item)
        {
            throw new NotImplementedException();
        }

        public async Task<ValidationResult> ValidateAsync(FindAccountCoursePaymentsQuery item)
        {
            var validationResult = new ValidationResult();

            if (item.AccountId == 0)
            {
                validationResult.AddError(nameof(item.AccountId), "Account ID has not been supplied");
            }

            if (item.UkPrn == 0)
            {
                validationResult.AddError(nameof(item.UkPrn), "UkPrn has not been supplied");
            }

            if (string.IsNullOrEmpty(item.CourseName))
            {
                validationResult.AddError(nameof(item.CourseName), "Course name has not been supplied");
            }

            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                validationResult.AddError(nameof(item.ExternalUserId), "External user ID has not been supplied");
            }

            if (item.FromDate == DateTime.MinValue)
            {
                validationResult.AddError(nameof(item.FromDate), "From date has not been supplied");
            }

            if (item.ToDate == DateTime.MinValue)
            {
                validationResult.AddError(nameof(item.ToDate), "To date has not been supplied");
            }

            if (!validationResult.IsValid())
                return validationResult;

            var memberView = await _membershipRepository.GetCaller(item.AccountId, item.ExternalUserId);

            if (memberView != null)
                return validationResult;

            validationResult.AddError("Membership", "User is not a member of this Account");
            validationResult.IsUnauthorized = true;

            return validationResult;
        }
    }
}

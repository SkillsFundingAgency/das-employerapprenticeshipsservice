using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreement;

public class GetEmployerAgreementQueryValidator : IValidator<GetEmployerAgreementRequest>
{
    public ValidationResult Validate(GetEmployerAgreementRequest item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetEmployerAgreementRequest item)
    {
        return await Task.Run(() =>
        {
            var validationResult = new ValidationResult();

            if (string.IsNullOrEmpty(item.AgreementId))
            {
                validationResult.AddError(nameof(item.AgreementId));
            }

            if (string.IsNullOrEmpty(item.ExternalUserId))
            {
                validationResult.AddError(nameof(item.ExternalUserId));
            }

            if (string.IsNullOrEmpty(item.HashedAccountId))
            {
                validationResult.AddError(nameof(item.HashedAccountId));
            }

            return validationResult;
        });
    }
}
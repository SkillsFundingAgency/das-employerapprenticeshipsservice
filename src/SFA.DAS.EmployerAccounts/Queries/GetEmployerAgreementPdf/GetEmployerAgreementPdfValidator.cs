using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;

public class GetEmployerAgreementPdfValidator : IValidator<GetEmployerAgreementPdfRequest>
{
    private readonly IMembershipRepository _membershipRepository;

    public GetEmployerAgreementPdfValidator(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public ValidationResult Validate(GetEmployerAgreementPdfRequest item)
    {
        throw new System.NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetEmployerAgreementPdfRequest item)
    {
            
        var validationResult = new ValidationResult();

        if (item.AccountId <= 0)
        {
            validationResult.AddError(nameof(item.AccountId));
        }

        if (item.LegalAgreementId <= 0)
        {
            validationResult.AddError(nameof(item.LegalAgreementId));
        }

        if (string.IsNullOrEmpty(item.UserId))
        {
            validationResult.AddError(nameof(item.UserId));
        }

        if (!validationResult.IsValid())
        {
            return validationResult;
        }

        var member = await _membershipRepository.GetCaller(item.AccountId, item.UserId);

        if (member == null || member.Role != Role.Owner)
        {
            validationResult.IsUnauthorized = true;
        }

        return validationResult;
    }
}
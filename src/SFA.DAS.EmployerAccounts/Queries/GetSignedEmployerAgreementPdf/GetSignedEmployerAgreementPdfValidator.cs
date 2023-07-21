using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;

public class GetSignedEmployerAgreementPdfValidator : IValidator<GetSignedEmployerAgreementPdfRequest>
{
    private readonly IMembershipRepository _membershipRepository;

    public GetSignedEmployerAgreementPdfValidator(IMembershipRepository membershipRepository)
    {
        _membershipRepository = membershipRepository;
    }

    public ValidationResult Validate(GetSignedEmployerAgreementPdfRequest item)
    {
        throw new NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(GetSignedEmployerAgreementPdfRequest item)
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
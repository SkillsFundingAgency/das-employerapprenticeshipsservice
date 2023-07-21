using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Data.Contracts;

namespace SFA.DAS.EmployerAccounts.Commands.CreateAccount;

public class CreateAccountCommandValidator : IValidator<CreateAccountCommand>
{
    private readonly IEmployerSchemesRepository _employerSchemesRepository;

    public CreateAccountCommandValidator(IEmployerSchemesRepository employerSchemesRepository)
    {
        _employerSchemesRepository = employerSchemesRepository;
    }

    public ValidationResult Validate(CreateAccountCommand item)
    {
        throw new System.NotImplementedException();
    }

    public async Task<ValidationResult> ValidateAsync(CreateAccountCommand item)
    {
        var validationResult = new ValidationResult();

        if (string.IsNullOrWhiteSpace(item.ExternalUserId))
            validationResult.AddError("UserId", "No UserId supplied");

        if ((item.OrganisationType == OrganisationType.CompaniesHouse || item.OrganisationType == OrganisationType.Charities || item.OrganisationType == OrganisationType.PensionsRegulator) &&
            string.IsNullOrWhiteSpace(item.OrganisationReferenceNumber))
            validationResult.AddError(nameof(item.OrganisationReferenceNumber), "No organisation reference number supplied");

        if (string.IsNullOrWhiteSpace(item.OrganisationName))
            validationResult.AddError(nameof(item.OrganisationName), "No organisation name supplied");

        if (string.IsNullOrWhiteSpace(item.PayeReference))
            validationResult.AddError(nameof(item.EmployerRefName), "No employer reference name supplied");

        if (validationResult.IsValid())
        {
            var result = await _employerSchemesRepository.GetSchemeByRef(item.PayeReference);
            if (result != null)
            {
                validationResult.AddError(nameof(item.PayeReference),"Scheme already in use");
            }
        }

        return validationResult;
    }
}
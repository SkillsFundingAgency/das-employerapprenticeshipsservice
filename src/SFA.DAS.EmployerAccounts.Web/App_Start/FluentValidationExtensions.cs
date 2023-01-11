
using SFA.DAS.EmployerAccounts.Web.Validation;

namespace SFA.DAS.EmployerAccounts.Web;

public static class FluentValidationExtensions
{
    public static void AddFluentValidation(this IServiceCollection services)
    {
        services.AddScoped<IValidator<OrganisationDetailsViewModel>,OrganisationDetailsViewModelValidator>();
    }
}
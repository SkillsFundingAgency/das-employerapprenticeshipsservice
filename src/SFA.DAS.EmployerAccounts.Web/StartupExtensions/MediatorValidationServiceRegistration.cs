using SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount;
using SFA.DAS.EmployerAccounts.Commands.CreateAccount;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;
using SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;
using SFA.DAS.EmployerAccounts.Validation;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class MediatorValidationServiceRegistration
{
    public static IServiceCollection AddMediatorValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<GetEmployerAccountByHashedIdQuery>, GetEmployerAccountByHashedIdValidator>();
        services.AddTransient<IValidator<CreateAccountCommand>, CreateAccountCommandValidator>();
        services.AddTransient<IValidator<CreateUserAccountCommand>, CreateUserAccountCommandValidator>();
        services.AddTransient<IValidator<CreateLegalEntityCommand>, CreateLegalEntityValidator>();
        services.AddTransient<IValidator<AddPayeToAccountCommand>, AddPayeToAccountCommandValidator>();
        services.AddTransient<IValidator<RenameEmployerAccountCommand>, RenameEmployerAccountCommandValidator>();
        services.AddTransient<IValidator<GetEmployerEnglishFractionHistoryQuery>, GetEmployerEnglishFractionHistoryQueryValidator>();
        services.AddTransient<IValidator<GetPayeSchemeByRefQuery>, GetPayeSchemeByRefValidator>();
        services.AddTransient<IValidator<RemovePayeFromAccountCommand>, RemovePayeFromAccountCommandValidator>();
        services.AddTransient<IValidator<GetAccountPayeSchemesForAuthorisedUserQuery>, GetAccountPayeSchemesForAuthorisedUserQueryValidator>();

        return services;
    }
}
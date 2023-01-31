using SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount;
using SFA.DAS.EmployerAccounts.Commands.CreateAccount;
using SFA.DAS.EmployerAccounts.Commands.CreateInvitation;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;
using SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember;
using SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;
using SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;
using SFA.DAS.EmployerAccounts.Commands.UpdateShowWizard;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetAccountStats;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetUser;
using SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;
using SFA.DAS.EmployerAccounts.Validation;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class MediatorValidationServiceRegistration
{
    public static IServiceCollection AddMediatorCommandValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<CreateAccountCommand>, CreateAccountCommandValidator>();
        services.AddTransient<IValidator<CreateUserAccountCommand>, CreateUserAccountCommandValidator>();
        services.AddTransient<IValidator<CreateLegalEntityCommand>, CreateLegalEntityValidator>();
        services.AddTransient<IValidator<AddPayeToAccountCommand>, AddPayeToAccountCommandValidator>();
        services.AddTransient<IValidator<RenameEmployerAccountCommand>, RenameEmployerAccountCommandValidator>();
        services.AddTransient<IValidator<RemovePayeFromAccountCommand>, RemovePayeFromAccountCommandValidator>();
        services.AddTransient<IValidator<RemoveLegalEntityCommand>, RemoveLegalEntityCommandValidator>();
        services.AddTransient<IValidator<SignEmployerAgreementCommand>, SignEmployerAgreementCommandValidator>();
        services.AddTransient<IValidator<RemoveTeamMemberCommand>, RemoveTeamMemberCommandValidator>();
        services.AddTransient<IValidator<UpdateShowAccountWizardCommand>, UpdateShowAccountWizardCommandValidator>();
        services.AddTransient<IValidator<CreateInvitationCommand>, CreateInvitationCommandValidator>();
        
        return services;
    }

    public static IServiceCollection AddMediatorQueryValidators(this IServiceCollection services)
    {
        services.AddTransient<IValidator<GetEmployerAccountByHashedIdQuery>, GetEmployerAccountByHashedIdValidator>();
        services.AddTransient<IValidator<GetEmployerEnglishFractionHistoryQuery>, GetEmployerEnglishFractionHistoryQueryValidator>();
        services.AddTransient<IValidator<GetPayeSchemeByRefQuery>, GetPayeSchemeByRefValidator>();
        services.AddTransient<IValidator<GetAccountPayeSchemesForAuthorisedUserQuery>, GetAccountPayeSchemesForAuthorisedUserQueryValidator>();
        services.AddTransient<IValidator<GetAccountLegalEntityRemoveRequest>, GetAccountLegalEntityRemoveValidator>();
        services.AddTransient<IValidator<GetEmployerAgreementPdfRequest>, GetEmployerAgreementPdfValidator>();
        services.AddTransient<IValidator<GetOrganisationAgreementsRequest>, GetOrganisationAgreementsValidator>();
        services.AddTransient<IValidator<GetSignedEmployerAgreementPdfRequest>, GetSignedEmployerAgreementPdfValidator>();
        services.AddTransient<IValidator<GetAccountEmployerAgreementsRequest>, GetAccountEmployerAgreementsValidator>();
        services.AddTransient<IValidator<GetEmployerAgreementRequest>, GetEmployerAgreementQueryValidator>();
        services.AddTransient<IValidator<GetNextUnsignedEmployerAgreementRequest>, GetNextUnsignedEmployerAgreementValidator>();
        services.AddTransient<IValidator<GetAccountStatsQuery>, GetAccountStatsQueryValidator>();
        services.AddTransient<IValidator<GetAccountTeamMembersQuery>, GetAccountTeamMembersValidator>();
        services.AddTransient<IValidator<GetUserQuery>, GetUserQueryValidator>();
        services.AddTransient<IValidator<GetUserByRefQuery>, GetUserByRefQueryValidator>();
        services.AddTransient<IValidator<GetAccountTasksQuery>, GetAccountTasksQueryValidator>();

        return services;
    }
}
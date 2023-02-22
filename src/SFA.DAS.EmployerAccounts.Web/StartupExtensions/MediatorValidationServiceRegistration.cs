using System.Reflection;
using SFA.DAS.EmployerAccounts.Commands.AcceptInvitation;
using SFA.DAS.EmployerAccounts.Commands.AddPayeToAccount;
using SFA.DAS.EmployerAccounts.Commands.CreateAccount;
using SFA.DAS.EmployerAccounts.Commands.CreateInvitation;
using SFA.DAS.EmployerAccounts.Commands.CreateLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.CreateOrganisationAddress;
using SFA.DAS.EmployerAccounts.Commands.CreateUserAccount;
using SFA.DAS.EmployerAccounts.Commands.DismissMonthlyTaskReminder;
using SFA.DAS.EmployerAccounts.Commands.RemoveLegalEntity;
using SFA.DAS.EmployerAccounts.Commands.RemoveTeamMember;
using SFA.DAS.EmployerAccounts.Commands.RenameEmployerAccount;
using SFA.DAS.EmployerAccounts.Commands.SignEmployerAgreement;
using SFA.DAS.EmployerAccounts.Commands.UnsubscribeNotification;
using SFA.DAS.EmployerAccounts.Commands.UpdateOrganisationDetails;
using SFA.DAS.EmployerAccounts.Commands.UpdateShowWizard;
using SFA.DAS.EmployerAccounts.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EmployerAccounts.Commands.UpsertRegisteredUser;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntities;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntity;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;
using SFA.DAS.EmployerAccounts.Queries.GetAccountPayeSchemes;
using SFA.DAS.EmployerAccounts.Queries.GetAccountStats;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTasks;
using SFA.DAS.EmployerAccounts.Queries.GetAccountTeamMembers;
using SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccount;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerEnglishFractionHistory;
using SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationAgreements;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationById;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisations;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationsByAorn;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeByRef;
using SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;
using SFA.DAS.EmployerAccounts.Queries.GetReservations;
using SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.Queries.GetSingleCohort;
using SFA.DAS.EmployerAccounts.Queries.GetUser;
using SFA.DAS.EmployerAccounts.Queries.GetUserAccountRole;
using SFA.DAS.EmployerAccounts.Queries.GetUserInvitations;
using SFA.DAS.EmployerAccounts.Queries.GetUserNotificationSettings;
using SFA.DAS.EmployerAccounts.Queries.GetVacancies;
using SFA.DAS.EmployerAccounts.Queries.RemovePayeFromAccount;
using SFA.DAS.EmployerAccounts.Validation;

namespace SFA.DAS.EmployerAccounts.Web.StartupExtensions;

public static class MediatorValidationServiceRegistration
{
    public static IServiceCollection AddMediatorCommandValidators(this IServiceCollection services)
    {
        // Get the assembly that contains the validator type
        Assembly validatorAssembly = typeof(CreateAccountCommandValidator).Assembly;

        // Register all validators in the assembly
        var validatorTypes = validatorAssembly
            .GetTypes()
            .Where(t => t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>)));

        foreach (var validatorType in validatorTypes)
        {
            var validatorInterface = validatorType.GetInterfaces().Single(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IValidator<>));
            services.AddTransient(validatorInterface, validatorType);
        }

        //services.AddTransient<IValidator<CreateAccountCommand>, CreateAccountCommandValidator>();
        //services.AddTransient<IValidator<CreateUserAccountCommand>, CreateUserAccountCommandValidator>();
        //services.AddTransient<IValidator<CreateLegalEntityCommand>, CreateLegalEntityValidator>();
        //services.AddTransient<IValidator<AddPayeToAccountCommand>, AddPayeToAccountCommandValidator>();
        //services.AddTransient<IValidator<RenameEmployerAccountCommand>, RenameEmployerAccountCommandValidator>();
        //services.AddTransient<IValidator<RemovePayeFromAccountCommand>, RemovePayeFromAccountCommandValidator>();
        //services.AddTransient<IValidator<RemoveLegalEntityCommand>, RemoveLegalEntityCommandValidator>();
        //services.AddTransient<IValidator<SignEmployerAgreementCommand>, SignEmployerAgreementCommandValidator>();
        //services.AddTransient<IValidator<RemoveTeamMemberCommand>, RemoveTeamMemberCommandValidator>();
        //services.AddTransient<IValidator<UpdateShowAccountWizardCommand>, UpdateShowAccountWizardCommandValidator>();
        //services.AddTransient<IValidator<CreateInvitationCommand>, CreateInvitationCommandValidator>();
        //services.AddTransient<IValidator<UpsertRegisteredUserCommand>, UpsertRegisteredUserCommandValidator>();
        //services.AddTransient<IValidator<AcceptInvitationCommand>, AcceptInvitationCommandValidator>();
        //services.AddTransient<IValidator<UpdateOrganisationDetailsCommand>, UpdateOrganisationDetailsCommandValidator>();
        //services.AddTransient<IValidator<DismissMonthlyTaskReminderCommand>, DismissMonthlyTaskReminderCommandValidator>();
        //services.AddTransient<IValidator<UnsubscribeNotificationCommand>, UnsubscribeNotificationValidator>();
        //services.AddTransient<IValidator<UpdateUserNotificationSettingsCommand>, UpdateUserNotificationSettingsValidator>();
        
        return services;
    }

    public static IServiceCollection AddMediatorQueryValidators(this IServiceCollection services)
    {
        //services.AddTransient<IValidator<GetEmployerAccountByIdQuery>, GetEmployerAccountByIdValidator>();
        //services.AddTransient<IValidator<GetEmployerEnglishFractionHistoryQuery>, GetEmployerEnglishFractionHistoryQueryValidator>();
        //services.AddTransient<IValidator<GetPayeSchemeByRefQuery>, GetPayeSchemeByRefValidator>();
        //services.AddTransient<IValidator<GetAccountPayeSchemesForAuthorisedUserQuery>, GetAccountPayeSchemesForAuthorisedUserQueryValidator>();
        //services.AddTransient<IValidator<GetAccountLegalEntityRemoveRequest>, GetAccountLegalEntityRemoveValidator>();
        //services.AddTransient<IValidator<GetEmployerAgreementPdfRequest>, GetEmployerAgreementPdfValidator>();
        //services.AddTransient<IValidator<GetOrganisationAgreementsRequest>, GetOrganisationAgreementsValidator>();
        //services.AddTransient<IValidator<GetSignedEmployerAgreementPdfRequest>, GetSignedEmployerAgreementPdfValidator>();
        //services.AddTransient<IValidator<GetAccountEmployerAgreementsRequest>, GetAccountEmployerAgreementsValidator>();
        //services.AddTransient<IValidator<GetEmployerAgreementRequest>, GetEmployerAgreementQueryValidator>();
        //services.AddTransient<IValidator<GetNextUnsignedEmployerAgreementRequest>, GetNextUnsignedEmployerAgreementValidator>();
        //services.AddTransient<IValidator<GetAccountStatsQuery>, GetAccountStatsQueryValidator>();
        //services.AddTransient<IValidator<GetAccountTeamMembersQuery>, GetAccountTeamMembersValidator>();
        //services.AddTransient<IValidator<GetUserQuery>, GetUserQueryValidator>();
        //services.AddTransient<IValidator<GetUserByRefQuery>, GetUserByRefQueryValidator>();
        //services.AddTransient<IValidator<GetAccountTasksQuery>, GetAccountTasksQueryValidator>();
        //services.AddTransient<IValidator<GetApprenticeshipsRequest>, GetApprenticeshipsValidator>();
        //services.AddTransient<IValidator<GetReservationsRequest>, GetReservationsRequestValidator>();
        //services.AddTransient<IValidator<GetSingleCohortRequest>, GetSingleCohortRequestValidator>();
        //services.AddTransient<IValidator<GetVacanciesRequest>, GetVacanciesRequestValidator>();
        //services.AddTransient<IValidator<GetUserAccountRoleQuery>, GetUserAccountRoleValidator>();
        //services.AddTransient<IValidator<GetHmrcEmployerInformationQuery>, GetHmrcEmployerInformationValidator>();
        //services.AddTransient<IValidator<GetNumberOfUserInvitationsQuery>, GetNumberOfUserInvitationsValidator>();
        //services.AddTransient<IValidator<GetAccountLegalEntityRequest>, GetAccountLegalEntityValidator>();
        //services.AddTransient<IValidator<GetOrganisationByIdRequest>, GetOrganisationByIdValidator>();
        //services.AddTransient<IValidator<CreateOrganisationAddressRequest>, CreateOrganisationAddressValidator>();
        //services.AddTransient<IValidator<GetAccountLegalEntitiesRequest>, GetAccountLegalEntitiesValidator>();
        //services.AddTransient<IValidator<GetOrganisationsRequest>, GetOrganisationsValidator>();
        //services.AddTransient<IValidator<GetOrganisationsByAornRequest>, GetOrganisationsByAornValidator>();
        //services.AddTransient<IValidator<GetPensionRegulatorRequest>, GetPensionRegulatorValidator>();
        //services.AddTransient<IValidator<GetUserNotificationSettingsQuery>, GetUserNotificationSettingsQueryValidator>();
        
        return services;
    }
}
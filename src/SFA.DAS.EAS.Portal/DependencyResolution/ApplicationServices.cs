using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Adapters;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Commands.Cohort;
using SFA.DAS.EAS.Portal.Application.Commands.Reservation;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<AddReservationCommand>();
            var serviceProvider = services.BuildServiceProvider();
            var configuration = serviceProvider.GetService<IConfiguration>();

            services.AddCommitmentsApiConfiguration(configuration);

            // hashing service config            
            var hashServiceConfig = configuration.GetPortalSection<HashingServiceConfiguration>(PortalSections.HashingService);            
            services.AddSingleton<IHashingService>(s => new HashingService.HashingService(hashServiceConfig.AccountLegalEntityPublicAllowedCharacters, hashServiceConfig.AccountLegalEntityPublicHashstring));
            
            services.AddScoped<IMessageContext, MessageContext>();            
            services.AddTransient<IAccountDocumentService, AccountDocumentService>();                        
            services.Decorate<IAccountDocumentService, AccountDocumentServiceWithSetProperties>();
            services.Decorate<IAccountDocumentService, AccountDocumentServiceWithDuplicateCheck>();
            services.AddTransient<IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand>, CohortAdapter>();

            // Register all ICommandHandler<> types
            services.Scan(scan =>
                    scan.FromAssembliesOf(typeof(ICommandHandler<>))
                    .AddClasses(classes =>
                    classes.AssignableTo(typeof(ICommandHandler<>)).Where(_ => !_.IsGenericType))
                        .AsImplementedInterfaces()
                        .WithTransientLifetime());

            return services;
        }
    }
}
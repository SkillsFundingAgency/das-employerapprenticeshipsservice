using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
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

            services.Scan(scan =>
                    scan.FromAssembliesOf(typeof(ICommand<>))
                    .AddClasses(classes =>
                    classes.AssignableTo(typeof(ICommand<>)).Where(_ => !_.IsGenericType))
                        .AsImplementedInterfaces()
                        .WithTransientLifetime());

            return services;
        }
    }
}
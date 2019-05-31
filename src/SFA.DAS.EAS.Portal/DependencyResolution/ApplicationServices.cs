using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Services;
using SFA.DAS.EAS.Portal.Configuration;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            //todo: AddHashingServices
            // hashing service config            
            var hashServiceConfig = hostBuilderContext.Configuration.GetPortalSection<HashingServiceConfiguration>(PortalSections.HashingService);
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
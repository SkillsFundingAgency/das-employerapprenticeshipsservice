using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SFA.DAS.EAS.Portal.Application.EventHandlers;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, HostBuilderContext hostBuilderContext)
        {
            services.AddScoped<IMessageContext, MessageContext>();            
            services.AddScoped<IMessageContextInitialisation, MessageContext>(sp => (MessageContext)sp.GetService<IMessageContext>());
            services.AddTransient<IAccountDocumentService, AccountDocumentService>();                        
            services.Decorate<IAccountDocumentService, AccountDocumentServiceWithSetProperties>();
            services.Decorate<IAccountDocumentService, AccountDocumentServiceWithDuplicateCheck>();

            services.Scan(scan =>
                    scan.FromAssembliesOf(typeof(IEventHandler<>))
                    .AddClasses(classes =>
                    classes.AssignableTo(typeof(IEventHandler<>)).Where(_ => !_.IsGenericType))
                        .AsImplementedInterfaces()
                        .WithTransientLifetime());

            return services;
        }
    }
}
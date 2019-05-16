using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.DataLock;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Adapters;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Commands.Cohort;
using SFA.DAS.EAS.Portal.Application.Commands.Reservation;
using SFA.DAS.EAS.Portal.Application.Services;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddTransient<AddReservationCommand>();
            var configuration = services.BuildServiceProvider().GetService<IConfiguration>();

            services.AddCommitmentsApiConfiguration(configuration);

            services.AddScoped<IMessageContext, MessageContext>();            
            services.AddTransient<IAccountDocumentService, AccountDocumentService>();            
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
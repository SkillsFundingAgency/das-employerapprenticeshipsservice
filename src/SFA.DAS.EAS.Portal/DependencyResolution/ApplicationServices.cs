using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Adapters;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Application.Commands.Cohort;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //return services.AddTransient<IAddReserveFundingCommand, AddReserveFundingCommand>();
            services.AddTransient<AddReserveFundingCommand>();

            services.AddTransient<ICommandHandler<CohortApprovalRequestedCommand>, CohortApprovalRequestedCommandHandler>();
            services.AddTransient<IAdapter<CohortApprovalRequestedByProvider, CohortApprovalRequestedCommand>, CohortAdapter>();
            return services;
        }
    }
}
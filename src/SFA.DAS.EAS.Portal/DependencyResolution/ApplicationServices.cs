using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Portal.Application.Commands;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //todo: 2 interfaces, 1 with props, 1 with methods!?
            //return services.AddTransient<IAddReserveFundingCommand, AddReservationCommand>();
            return services.AddTransient<AddReservationCommand>();
        }
    }
}
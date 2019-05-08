using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Portal.Application.Commands;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //todo: singleton in similar way to client?
            //todo: 2 interfaces, 1 with props, 1 with methods!?
            //return services.AddTransient<IAddReserveFundingCommand, AddReserveFundingCommand>();
            return services.AddTransient<AddReserveFundingCommand>();
        }
    }
}
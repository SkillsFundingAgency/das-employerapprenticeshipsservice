using Microsoft.Extensions.DependencyInjection;
using SFA.DAS.EAS.Portal.Application.Commands;

namespace SFA.DAS.EAS.Portal.DependencyResolution
{
    public static class ApplicationServices
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            return services.AddTransient<AddReserveFundingCommand>();
        }
    }
}
//using Microsoft.Extensions.DependencyInjection;
//using SFA.DAS.EAS.Portal.Database;

//namespace SFA.DAS.EAS.Portal.Jobs.DependencyResolution
//{
//    //not sure which project this will end up in yet
//    public static class DatabaseServices
//    {
//        public static IServiceCollection AddDatabaseServices(this IServiceCollection services)
//        {
//            // if we have to move this out of the host into a shared assembly, then we'll a mechanism to keep this registration to ourselves, e.g. named
//            return services
//                .AddTransient<IDocumentClientFactory>()
//                .AddTransient(sp => sp.GetService<IDocumentClientFactory>().CreateDocumentClient());
//        }
//    }
//}
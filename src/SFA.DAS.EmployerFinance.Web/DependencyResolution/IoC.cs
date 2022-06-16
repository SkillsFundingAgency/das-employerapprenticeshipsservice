using SFA.DAS.Authorization.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.EmployerFeatures.DependencyResolution.StructureMap;
using SFA.DAS.Authorization.EmployerUserRoles.DependencyResolution.StructureMap;
using SFA.DAS.EmployerFinance.Api.Client;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.DependencyResolution;
using SFA.DAS.UnitOfWork.EntityFramework.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.DependencyResolution.StructureMap;
using SFA.DAS.UnitOfWork.NServiceBus.Features.ClientOutbox.DependencyResolution.StructureMap;
using StructureMap;

namespace SFA.DAS.EmployerFinance.Web.DependencyResolution
{
    public static class IoC
    {
        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<AccountApiClientRegistry>();
                c.AddRegistry<AuthorizationRegistry>();
                c.AddRegistry<CachesRegistry>();
                c.AddRegistry<ConfigurationRegistry>();
                c.AddRegistry<DateTimeRegistry>();
                c.AddRegistry<DataRegistry>();
                c.AddRegistry(new EmployerFinanceApiClientRegistry(context => context.GetInstance<EmployerFinanceConfiguration>().EmployerFinanceApi));
                c.AddRegistry<EntityFrameworkUnitOfWorkRegistry<EmployerFinanceDbContext>>();
                c.AddRegistry<HashingRegistry>();
                c.AddRegistry<LoggerRegistry>();
                c.AddRegistry<MapperRegistry>();
                c.AddRegistry<MediatorRegistry>();
                c.AddRegistry<NServiceBusClientUnitOfWorkRegistry>();
                c.AddRegistry<NServiceBusUnitOfWorkRegistry>();
                c.AddRegistry<TokenServiceRegistry>();
                c.AddRegistry<StartupRegistry>();
                c.AddRegistry<DefaultRegistry>();
                c.AddRegistry<EmployerFeaturesAuthorizationRegistry>();
                c.AddRegistry<EmployerUserRolesAuthorizationRegistry>();
                c.AddRegistry<ContentApiClientRegistry>();
                c.AddRegistry<ManageApprenticeshipsOuterApiRegistry>();
                c.AddRegistry<CommitmentsV2ApiClientRegistry>();
            });
        }
    }
}
using MediatR;
using Moq;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Web;
using SFA.DAS.EAS.Web.Authentication;
using StructureMap;
using StructureMap.Graph;

namespace SFA.DAS.EAS.TestCommon.DependencyResolution
{
    public class DefaultRegistry : Registry
    {

        public DefaultRegistry(Mock<IOwinWrapper> owinWrapperMock, Mock<ICookieService> cookieServiceMock)
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
                
            });
            
            For<IUserRepository>().Use<UserRepository>();

            For<IOwinWrapper>().Use(() => owinWrapperMock.Object);

            For<ICookieService>().Use(() => cookieServiceMock.Object);
            
            For<IConfiguration>().Use<EmployerApprenticeshipsServiceConfiguration>();
            
            AddMediatrRegistrations();
        }

        private void AddMediatrRegistrations()
        {
            For<SingleInstanceFactory>().Use<SingleInstanceFactory>(ctx => t => ctx.GetInstance(t));
            For<MultiInstanceFactory>().Use<MultiInstanceFactory>(ctx => t => ctx.GetAllInstances(t));

            For<IMediator>().Use<Mediator>();
        }

    }
}

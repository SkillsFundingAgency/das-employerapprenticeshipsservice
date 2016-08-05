using MediatR;
using Moq;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Data;
using SFA.DAS.EmployerApprenticeshipsService.Infrastructure.Data;
using SFA.DAS.EmployerApprenticeshipsService.Web.Authentication;
using StructureMap;
using StructureMap.Graph;

namespace SFA.DAS.EmployerApprenticeshipsService.Web.AcceptanceTests.DependencyResolution
{
    public class DefaultRegistry : Registry
    {

        public DefaultRegistry(Mock<IOwinWrapper> owinWrapperMock)
        {
            Scan(scan =>
            {
                scan.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS.EmployerApprenticeshipsService"));
                scan.RegisterConcreteTypesAgainstTheFirstInterface();
                
            });
            
            For<IUserRepository>().Use<FileSystemUserRepository>();

            For<IOwinWrapper>().Use(() => owinWrapperMock.Object);
            
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

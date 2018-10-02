using HMRC.ESFA.Levy.Api.Client;
using Moq;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.Events.Api.Client;
using StructureMap;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.DependencyResolution
{
    public class MockRegisterations : Registry
    {
        public MockRegisterations()
        {
            AddMock<IApprenticeshipLevyApiClient>();
            AddMock<IAuthenticationService>();
            AddMock<IAuthorizationService>();
            AddMock<ICurrentDateTime>();
            AddMock<IEmployerAccountRepository>();
            AddMock<IEventsApi>();
            AddMock<IMembershipRepository>();
            AddMock<IPayeRepository>();
        }

        private void AddMock<TMock>() where TMock : class
        {
            var mock = new Mock<TMock>();
            For<TMock>().Use(mock.Object);
            For<IMock<TMock>>().Use(mock);
        }
    }
}
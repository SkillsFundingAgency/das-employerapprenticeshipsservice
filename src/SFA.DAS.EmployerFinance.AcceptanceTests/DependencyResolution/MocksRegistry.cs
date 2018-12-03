﻿using HMRC.ESFA.Levy.Api.Client;
using Moq;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.Events.Api.Client;
using StructureMap;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.DependencyResolution
{
    public class MocksRegistry : Registry
    {
        public MocksRegistry()
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

        private void AddMock<T>() where T : class
        {
            var mock = new Mock<T>();

            For<IMock<T>>().Use(mock);
            For<Mock<T>>().Use(mock);
            For<T>().Use(mock.Object);
        }
    }
}
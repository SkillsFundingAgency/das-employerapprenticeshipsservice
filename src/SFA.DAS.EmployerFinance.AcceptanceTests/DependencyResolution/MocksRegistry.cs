﻿using System;
using HMRC.ESFA.Levy.Api.Client;
using Moq;
using SFA.DAS.Authentication;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.TokenService.Api.Client;
using SFA.DAS.TokenService.Api.Types;
using StructureMap;

namespace SFA.DAS.EmployerFinance.AcceptanceTests.DependencyResolution
{
    public class MocksRegistry : Registry
    {
        public MocksRegistry()
        {
            AddMock<IApprenticeshipLevyApiClient>();
            AddMock<IAuthenticationService>();
            AddMock<IEmployerAccountRepository>();
            AddMock<IEventsApi>();
            AddMock<IPayeRepository>();
            SetupCurrentDateTimeMock(AddMock<ICurrentDateTime>());
            SetupTokenServiceApiClientMock(AddMock<ITokenServiceApiClient>());
            SetupAuthorizationServiceMock(AddMock<IAuthorizationService>());
        }

        private Mock<T> AddMock<T>() where T : class
        {
            var mock = new Mock<T>();

            For<IMock<T>>().Use(mock);
            For<Mock<T>>().Use(mock);
            For<T>().Use(mock.Object);
            return mock;
        }

        private void SetupAuthorizationServiceMock(Mock<IAuthorizationService> mockAuthorizationService)
        {
            mockAuthorizationService
                .Setup(
                    m => m.IsAuthorized(EmployerUserRole.Any))
                .Returns(true);
        }

        private void SetupTokenServiceApiClientMock(Mock<ITokenServiceApiClient> mock)
        {
            mock.Setup(c => c.GetPrivilegedAccessTokenAsync()).ReturnsAsync(new PrivilegedAccessToken());
        }

        private void SetupCurrentDateTimeMock(Mock<ICurrentDateTime> mock)
        {
            mock.Setup(x => x.Now).Returns(() => DateTime.Now);
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.Features
{
    [TestFixture]
    public class AuthorisationServiceTests
    {
        [TestCase(true)]
        [TestCase(true, true)]
        [TestCase(true, true, true)]
        [TestCase(false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, true, false, true)]
        public void IsOperationAuthorized_WhenICheckIfOperationIsAuthorized_ThenShouldReturnExpectedResult(bool expectedResult, params bool[] handlerResults)
        {
            // Arrange
            var fixtures = new OperationAuthorisationServiceTestFixtures();

            var authorizationService = fixtures
                .WithHandlerResults(handlerResults)
                .CreateAuthorisationService();

            //Act
            var result = authorizationService.IsOperationAuthorized();

            //Assert
            Assert.AreEqual(expectedResult, result);
        }
    }

    public class OperationAuthorisationServiceTestFixtures
    {
        public OperationAuthorisationServiceTestFixtures()
        {
            HandlerMocks = new List<Mock<IAuthorizationHandler>>();
            LogMock = new Mock<ILog>();
            FestureServiceMock = new Mock<IFeatureService>();
        }

        public IAuthorizationContext CreateAuthenticationContext()
        {
            return new AuthorizationContext { CurrentFeature = new Feature { Enabled = true } };
        }

        public IAuthorizationHandler[] Handlers =>
            HandlerMocks.Select(hm => hm.Object).ToArray();

        public List<Mock<IAuthorizationHandler>> HandlerMocks { get; }

        public Mock<ILog> LogMock { get; set; }
        public ILog Log => LogMock.Object;

        public Mock<IFeatureService> FestureServiceMock { get; set; }
        public IFeatureService FeatureService => FestureServiceMock.Object;
        public AuthorizationService CreateAuthorisationService()
        {
	        var dbContextMock = new Mock<EmployerAccountDbContext>();
            var authorizationContextCacheMock = new Mock<IAuthorizationContextCache>();
            var callerContextProvider = new Mock<ICallerContextProvider>();
            var configurationMock = new Mock<IConfigurationProvider>();
            
            authorizationContextCacheMock.Setup(c => c.GetAuthorizationContext()).Returns(new AuthorizationContext { CurrentFeature = new Feature { Enabled = true } });

			return new AuthorizationService(
				dbContextMock.Object,
				authorizationContextCacheMock.Object,
                Handlers,
				callerContextProvider.Object,
                configurationMock.Object,  
				FeatureService);
        }

        public OperationAuthorisationServiceTestFixtures WithHandlerResults(params bool[] handlerResults)
        {
            foreach (var handlerResult in handlerResults)
            {
                var newHandler = new Mock<IAuthorizationHandler>();

                newHandler
                    .Setup(h => h.CanAccessAsync(It.IsAny<IAuthorizationContext>()))
                    .ReturnsAsync(handlerResult);

                HandlerMocks.Add(newHandler);   
            }

            return this;
        }
    }
}

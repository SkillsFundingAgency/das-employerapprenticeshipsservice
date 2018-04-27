using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.Features;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Features;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Authorization
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
            var fixture = new OperationAuthorisationServiceTestFixture();

            var authorizationService = fixture
                .WithHandlerResults(handlerResults)
                .CreateAuthorisationService();

            //Act
            var result = authorizationService.IsAuthorized(fixture.Feature.FeatureType);

            //Assert
            Assert.AreEqual(expectedResult, result);
        }
    }

    public class OperationAuthorisationServiceTestFixture
    {
        public Feature Feature { get; set; }
        public Mock<IFeatureService> FeatureServiceMock { get; set; }
        public IFeatureService FeatureService => FeatureServiceMock.Object;
        public IAuthorizationHandler[] Handlers => HandlerMocks.Select(hm => hm.Object).ToArray();
        public List<Mock<IAuthorizationHandler>> HandlerMocks { get; }
        public Mock<ILog> LogMock { get; set; }
        public ILog Log => LogMock.Object;

        public OperationAuthorisationServiceTestFixture()
        {
            Feature = new Feature { Enabled = true, FeatureType = FeatureType.Test1 };
            FeatureServiceMock = new Mock<IFeatureService>();
            HandlerMocks = new List<Mock<IAuthorizationHandler>>();
            LogMock = new Mock<ILog>();
        }

        public AuthorizationService CreateAuthorisationService()
        {
            var authorizationContextCacheMock = new Mock<IAuthorizationContextCache>();
            var callerContextProvider = new Mock<ICallerContextProvider>();
            var configurationMock = new Mock<IConfigurationProvider>();
            var dbContextMock = new Mock<EmployerAccountDbContext>();

            authorizationContextCacheMock.Setup(c => c.GetAuthorizationContext()).Returns(new AuthorizationContext());
            FeatureServiceMock.Setup(f => f.GetFeature(Feature.FeatureType)).Returns(Feature);

            return new AuthorizationService(
				dbContextMock.Object,
				authorizationContextCacheMock.Object,
                Handlers,
				callerContextProvider.Object,
                configurationMock.Object,  
				FeatureService);
        }

        public OperationAuthorisationServiceTestFixture WithHandlerResults(params bool[] handlerResults)
        {
            foreach (var handlerResult in handlerResults)
            {
                var newHandler = new Mock<IAuthorizationHandler>();

                newHandler
                    .Setup(h => h.CanAccessAsync(It.IsAny<IAuthorizationContext>(), Feature))
                    .ReturnsAsync(handlerResult);

                HandlerMocks.Add(newHandler);   
            }

            return this;
        }
    }
}
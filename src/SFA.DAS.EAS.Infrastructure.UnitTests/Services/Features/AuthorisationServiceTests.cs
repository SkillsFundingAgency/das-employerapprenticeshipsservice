using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Authorization;
using SFA.DAS.EAS.Domain.Models.FeatureToggles;
using SFA.DAS.EAS.Infrastructure.Data;
using SFA.DAS.EAS.Infrastructure.Pipeline;
using SFA.DAS.EAS.Infrastructure.Services;
using SFA.DAS.EAS.Infrastructure.Services.Features;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.Features
{
    class AuthorisationServiceTests
    {
        [TestCase(true)]
        [TestCase(true, true)]
        [TestCase(true, true, true)]
        [TestCase(false, false)]
        [TestCase(false, true, false)]
        [TestCase(false, true, false, true)]
        public async Task CanAccessAsync_NoHandlers_ShouldReturnTrue(bool expectedResult, params bool[] handlerResults)
        {
            // Arrange
            var fixtures = new OperationAuthorisationServiceTestFixtures();

            var handler = fixtures
                .WithHandlerResults(handlerResults)
                .CreateAuthorisationService();

            var operationContext = fixtures.CreateOperationContext();

            //Act
            var result = await handler.CanAccessAsync(operationContext);

            //Assert
            Assert.AreEqual(expectedResult, expectedResult);
        }
    }

    class OperationAuthorisationServiceTestFixtures
    {
        public OperationAuthorisationServiceTestFixtures()
        {
            HandlerMocks = new List<Mock<IOperationAuthorisationHandler>>();
            LogMock = new Mock<ILog>();
            FestureServiceMock = new Mock<IFeatureService>();
        }

        public IAuthorizationContext CreateOperationContext()
        {
            return new AuthorizationContext { CurrentFeature = new Feature{Enabled = true}};
        }

        public IOperationAuthorisationHandler[] Handlers =>
            HandlerMocks.Select(hm => hm.Object).ToArray();

        public List<Mock<IOperationAuthorisationHandler>> HandlerMocks { get; }

        public Mock<ILog> LogMock { get; set; }
        public ILog Log => LogMock.Object;

        public Mock<IFeatureService> FestureServiceMock { get; set; }
        public IFeatureService FeatureService => FestureServiceMock.Object;
        public AuthorizationService CreateAuthorisationService()
        {
	        var dbContextMock = new Mock<EmployerAccountDbContext>();
	        var configurationMock = new Mock<IConfigurationProvider>();
	        var callerContextMock = new Mock<ICallerContext>();

			return new AuthorizationService(
				dbContextMock.Object, 
				configurationMock.Object,  
				callerContextMock.Object,
				FeatureService, 
				Log, 
				Handlers);
        }

        public OperationAuthorisationServiceTestFixtures WithHandlerResults(params bool[] handlerResults)
        {
            foreach (var handlerResult in handlerResults)
            {
                var newHandler = new Mock<IOperationAuthorisationHandler>();
                newHandler
                    .Setup(h => h.CanAccessAsync(It.IsAny<IAuthorizationContext>()))
                    .ReturnsAsync(handlerResult);

                HandlerMocks.Add(newHandler);    
            }
            return this;
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Domain.Models.Settings;
using SFA.DAS.EAS.Web.Orchestrators;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.EmployerAccountOrchestratorTests
{
    [TestFixture]
    public class WhenManagingUserSettings
    {
        private EmployerAccountOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private Mock<IHashingService> _hashingService;

        private EmployerApprenticeshipsServiceConfiguration _configuration;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _configuration = new EmployerApprenticeshipsServiceConfiguration();
            _hashingService = new Mock<IHashingService>();

            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(() => 123);

            _orchestrator = new EmployerAccountOrchestrator(_mediator.Object, _logger.Object, _cookieService.Object, _configuration, _hashingService.Object);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetUserNotificationSettingsQuery>()))
                .ReturnsAsync(new GetUserNotificationSettingsQueryResponse
                {
                    NotificationSettings = new List<UserNotificationSetting>()
                });

            _mediator.Setup(x => x.SendAsync(It.IsAny<UpdateUserNotificationSettingsCommand>()))
                .Returns(() => Task.FromResult(new Unit()));
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToRetrieveSettings()
        {
            //Act
            await _orchestrator.GetNotificationSettingsViewModel("", "USERREF");

            //Assert
            _mediator.Verify(x => x.SendAsync(
                It.Is<GetUserNotificationSettingsQuery>(s => s.AccountId == 123 && s.UserRef == "USERREF")),
                Times.Once);
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToUpdateSettings()
        {
            //Arrange
            var settings = new List<UserNotificationSetting>();

            //Act
            await _orchestrator.UpdateNotificationSettings("", "USERREF", settings);

            //Assert
            _mediator.Verify(x => x.SendAsync(
                It.Is<UpdateUserNotificationSettingsCommand>(
                    s=> s.AccountId == 123
                    && s.UserRef == "USERREF"
                    && s.Settings == settings)
                ), Times.Once);

        }
    }
}

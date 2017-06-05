using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Settings;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UpdateUserNotificationSettings
{
    [TestFixture]
    public class WhenUpdatingUserNotificationSettings
    {
        private UpdateUserNotificationSettingsCommandHandler _handler;
        private Mock<IAccountRepository> _repository;
        private Mock<IValidator<UpdateUserNotificationSettingsCommand>> _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<UpdateUserNotificationSettingsCommand>>();
            _validator.Setup(x => x.Validate(It.IsAny<UpdateUserNotificationSettingsCommand>()))
                .Returns(() => new ValidationResult());

            _repository = new Mock<IAccountRepository>();
            _repository.Setup(x => x.UpdateUserLegalEntitySettings(
                It.IsAny<string>(),
                It.IsAny<long>(),
                It.IsAny<List<UserNotificationSetting>>()
            )).Returns(() => Task.FromResult(new Unit()));

            _handler = new UpdateUserNotificationSettingsCommandHandler(_repository.Object, _validator.Object);
        }

        [Test]
        public async Task ThenTheCommandIsValidated()
        {
            //Arrange
            var command = new UpdateUserNotificationSettingsCommand
            {
                AccountId = 1,
                UserRef = "REF",
                Settings = new List<UserNotificationSetting>()
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _validator.Verify(x => x.Validate(It.IsAny<UpdateUserNotificationSettingsCommand>()), Times.Once);
        }

        [Test]
        public async Task ThenTheRepositoryIsCalledToUpdateSettings()
        {
            //Arrange
            var command = new UpdateUserNotificationSettingsCommand
            {
                AccountId = 1,
                UserRef = "REF",
                Settings = new List<UserNotificationSetting>()
            };

            //Act
            await _handler.Handle(command);

            //Assert
            _repository.Verify(x => x.UpdateUserLegalEntitySettings(
                It.Is<string>(s => s == "REF"),
                It.Is<long>(l => l == 1),
                It.IsAny<List<UserNotificationSetting>>()
                ), Times.Once);
        }
    }
}

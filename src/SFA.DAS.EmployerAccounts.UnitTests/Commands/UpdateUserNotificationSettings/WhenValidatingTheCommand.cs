using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.UpdateUserNotificationSettings;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.UpdateUserNotificationSettings
{
    [TestFixture]
    public class WhenValidatingTheCommand
    {
        private UpdateUserNotificationSettingsValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new UpdateUserNotificationSettingsValidator();
        }

        [Test]
        public void ThenUserRefMustBeSupplied()
        {
            //Arrange
            var command = new UpdateUserNotificationSettingsCommand
            {
                Settings = new List<UserNotificationSetting>()
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(command.UserRef)));
        }

        [Test]
        public void ThenSettingsMustBeSupplied()
        {
            //Arrange
            var command = new UpdateUserNotificationSettingsCommand
            {
                UserRef = "REF",
                Settings = null
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(command.Settings)));
        }

    }
}

﻿using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUserNotificationSettings;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserNotificationSettings
{
    [TestFixture]
    public class WhenIValidateTheRequest
    {
        private GetUserNotificationSettingsQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetUserNotificationSettingsQueryValidator();
        }

        [Test]
        public void ThenUserRefMustBeSupplied()
        {
            //Arrange
            var query = new GetUserNotificationSettingsQuery
            {
                AccountId = 1
            };

            //Act
            var result = _validator.Validate(query);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(query.UserRef)));
        }

        [Test]
        public void ThenAccountIdMustBeSupplied()
        {
            //Arrange
            var query = new GetUserNotificationSettingsQuery
            {
                UserRef = "TEST"
            };

            //Act
            var result = _validator.Validate(query);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(query.AccountId)));
        }
    }
}
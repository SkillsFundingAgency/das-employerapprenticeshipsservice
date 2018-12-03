﻿using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.UpdateShowWizard;

namespace SFA.DAS.EmployerAccounts.UnitTests.Commands.UpdateShowAccountWizardTests
{
    [TestFixture]
    public class WhenValidatingTheCommand
    {
        private UpdateShowAccountWizardCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new UpdateShowAccountWizardCommandValidator();
        }

        [Test]
        public void ThenCommandShouldBeValidIfAllPropertiesAreValid()
        {
            //Arrange
            var command = new UpdateShowAccountWizardCommand
            {
                HashedAccountId = "123ABC",
                ExternalUserId = "213FHG",
                ShowWizard = true
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenErrorsShouldBeReturnedIfPropertiesAreInValid()
        {
            //Arrange
            var command = new UpdateShowAccountWizardCommand();

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(command.HashedAccountId)));
            Assert.IsTrue(result.ValidationDictionary.ContainsKey(nameof(command.ExternalUserId)));
        }
    }
}

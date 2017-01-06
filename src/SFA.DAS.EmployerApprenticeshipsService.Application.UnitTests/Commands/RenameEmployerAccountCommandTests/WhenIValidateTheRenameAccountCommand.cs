using System;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RenameEmployerAccount;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RenameEmployerAccountCommandTests
{
    public class WhenIValidateTheRenameAccountCommand
    {
        private RenameEmployerAccountCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new RenameEmployerAccountCommandValidator();
        }

        [Test]
        public void ThenNewAccountNameCannotBeEmpty()
        {
            //Arrange
            var command = new RenameEmployerAccountCommand
            {
                NewName = String.Empty
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenNewAccountNameIsValidIfNotEmpty()
        {
            //Arrange
            var command = new RenameEmployerAccountCommand
            {
                NewName = "Test Renamed Account"
            };

            //Act
            var result = _validator.Validate(command);

            //Assert
            Assert.IsTrue(result.IsValid());
        }
    }
}

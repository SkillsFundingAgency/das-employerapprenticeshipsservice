using NUnit.Framework;

using SFA.DAS.EAS.Application.Commands.UndoApprenticeshipUpdate;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.UndoApprenticeshipUpdate
{
    [TestFixture]
    public class WhenValidatingCommand
    {
        private UndoApprenticeshipUpdateCommandValidator _validator;
        private UndoApprenticeshipUpdateCommand _command;

        [SetUp]
        public void Arrange()
        {
            _validator = new UndoApprenticeshipUpdateCommandValidator();

            _command = new UndoApprenticeshipUpdateCommand
            {
                ApprenticeshipId = 1,
                AccountId = 2,
                UserId = "tester"
            };
        }

        [Test]
        public void ThenApprenticeshipIdIsMandatory()
        {
            //Arrange
            _command.ApprenticeshipId = 0;

            //Act
            var result = _validator.Validate(_command);

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenProviderIdIsMandatory()
        {
            //Arrange
            _command.AccountId = 0;

            //Act
            var result = _validator.Validate(_command);

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenUserIdIsMandatory()
        {
            //Arrange
            _command.UserId = string.Empty;

            //Act
            var result = _validator.Validate(_command);

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenIfAllPropertiesAreProvidedThenIsValid()
        {
            //Act
            var result = _validator.Validate(_command);

            //Assert
            Assert.IsTrue(result.IsValid());
        }
    }
}

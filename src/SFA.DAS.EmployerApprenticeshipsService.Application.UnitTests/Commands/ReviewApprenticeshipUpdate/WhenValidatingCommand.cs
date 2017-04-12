using NUnit.Framework;

using SFA.DAS.EAS.Application.Commands.ReviewApprenticeshipUpdate;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.ReviewApprenticeshipUpdate
{
    [TestFixture]
    public class WhenValidatingCommand
    {
        private ReviewApprenticeshipUpdateCommandValidator _validator;
        private ReviewApprenticeshipUpdateCommand _command;

        [SetUp]
        public void Arrange()
        {
            _validator = new ReviewApprenticeshipUpdateCommandValidator();

            _command = new ReviewApprenticeshipUpdateCommand
            {
                ApprenticeshipId = 1,
                AccountId = 2,
                IsApproved = true,
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
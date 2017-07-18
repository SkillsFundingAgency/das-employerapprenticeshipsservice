using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.DeleteApprentice;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.DeleteApprenticeTests
{
    [TestFixture]
    public class WhenIValidateTheRequest
    {
        private DeleteApprenticeshipCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new DeleteApprenticeshipCommandValidator();
        }

        [Test]
        public void ThenAccountIdMustBeGreaterThanZero()
        {
            //Arrange
            var command = new DeleteApprenticeshipCommand
            {
                AccountId = 0,
                ApprenticeshipId = 1
            };

            //Act
            var actual = _validator.Validate(command);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenApprenticeshipIdMustBeGreaterThanZero()
        {
            //Arrange
            var command = new DeleteApprenticeshipCommand
            {
                AccountId = 1,
                ApprenticeshipId = 0
            };

            //Act
            var actual = _validator.Validate(command);

            //Assert
            Assert.IsFalse(actual.IsValid());
        }

        [Test]
        public void ThenTheRequestIsValidIfAllRequiredPropertiesAreProvided()
        {
            //Arrange
            var command = new DeleteApprenticeshipCommand
            {
                AccountId = 1,
                ApprenticeshipId = 1,
                UserId = "externalUserId"
            };

            //Act
            var actual = _validator.Validate(command);

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

    }
}

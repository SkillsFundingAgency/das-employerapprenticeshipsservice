using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetReservations;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetReservations
{
    public class WhenIValidateTheRequest 
    {
        private GetReservationsRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetReservationsRequestValidator();
        }

        [Test]
        public void ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = _validator.Validate(new GetReservationsRequest { AccountId = "123ABC", ExternalUserId = "user123" });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfNoAccountIdIsProvided()
        {
            //Act
            var result = _validator.Validate(new GetReservationsRequest { ExternalUserId = "user123" });

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfNoExternalUserIdIsProvided()
        {
            //Act
            var result = _validator.Validate(new GetReservationsRequest { AccountId = "123ABC" });

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfAccountIdIsEmpty()
        {
            //Act
            var result = _validator.Validate(new GetReservationsRequest { AccountId = string.Empty, ExternalUserId = "user123" });

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfExternalUserIdIsEmpty()
        {
            //Act
            var result = _validator.Validate(new GetReservationsRequest { AccountId = "123ABC", ExternalUserId = string.Empty });

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}

using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetVacancies;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetVacancies
{
    public class WhenIValidateTheRequest 
    {
        private GetVacanciesRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetVacanciesRequestValidator();
        }

        [Test]
        public void ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = _validator.Validate(new GetVacanciesRequest { AccountId = "123ABC", ExternalUserId = "user123" });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfNoAccountIdIsProvided()
        {
            //Act
            var result = _validator.Validate(new GetVacanciesRequest { ExternalUserId = "user123" });

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfNoExternalUserIdIsProvided()
        {
            //Act
            var result = _validator.Validate(new GetVacanciesRequest { AccountId = "123ABC" });

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfAccountIdIsEmpty()
        {
            //Act
            var result = _validator.Validate(new GetVacanciesRequest { AccountId = string.Empty, ExternalUserId = "user123" });

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfExternalUserIdIsEmpty()
        {
            //Act
            var result = _validator.Validate(new GetVacanciesRequest { AccountId = "123ABC", ExternalUserId = string.Empty });

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}

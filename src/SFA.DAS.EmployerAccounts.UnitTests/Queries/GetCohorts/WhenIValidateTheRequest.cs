using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetCohorts;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetCohorts
{
    public class WhenIValidateTheRequest
    {
        private GetCohortsRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetCohortsRequestValidator();
        }

        [Test]
        public void ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = _validator.Validate(new GetCohortsRequest {AccountId = 123 });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInValidIfAccountIdNotSet()
        {
            //Act
            var result = _validator.Validate(new GetCohortsRequest { });

            //Assert
            Assert.IsFalse(result.IsValid());
        }

    }
}

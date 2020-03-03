using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetAccountCohort;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountCohorts
{
    public class WhenIValidateTheRequest
    {
        private GetSingleCohortRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetSingleCohortRequestValidator();
        }

        [Test]
        public void ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = _validator.Validate(new GetSingleCohortRequest { HashedAccountId = "Abc123" });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInValidIfRequestIsNotValid()
        {
            //Act
            var result = _validator.Validate(new GetSingleCohortRequest { HashedAccountId = string.Empty });

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}

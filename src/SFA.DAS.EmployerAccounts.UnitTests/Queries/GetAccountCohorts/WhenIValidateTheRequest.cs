using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetAccountCohort;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountCohorts
{
    public class WhenIValidateTheRequest
    {
        private GetAccountCohortRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetAccountCohortRequestValidator();
        }

        [Test]
        public void ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = _validator.Validate(new GetAccountCohortRequest { HashedAccountId = "Abc123" });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInValidIfRequestIsNotValid()
        {
            //Act
            var result = _validator.Validate(new GetAccountCohortRequest { HashedAccountId = string.Empty });

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}

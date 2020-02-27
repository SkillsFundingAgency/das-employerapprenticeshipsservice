using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetApprenticeship
{
    public class WhenIValidateTheRequest
    {
        private GetApprenticeshipValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetApprenticeshipValidator();
        }

        [Test]
        public void ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = _validator.Validate(new GetApprenticeshipRequest { AccountId = 123 });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInValidIfRequestIsNotValid()
        {
            //Act
            var result = _validator.Validate(new GetApprenticeshipRequest { });

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }    
}

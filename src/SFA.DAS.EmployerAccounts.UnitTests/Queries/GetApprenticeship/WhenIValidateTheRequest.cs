using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetApprenticeship;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetApprenticeship
{
    public class WhenIValidateTheRequest
    {
        private GetApprenticeshipsValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetApprenticeshipsValidator();
        }

 
        [Test]
        public void ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = _validator.Validate(new GetApprenticeshipsRequest { AccountId = 4567, ExternalUserId = "user123" });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfNoAccountIdIsProvided()
        {
            //Act
            var result = _validator.Validate(new GetApprenticeshipsRequest { ExternalUserId = "user123" });

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInValidIfRequestIsNotValid()
        {
            //Act
            var result = _validator.Validate(new GetApprenticeshipsRequest { });

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }    
}

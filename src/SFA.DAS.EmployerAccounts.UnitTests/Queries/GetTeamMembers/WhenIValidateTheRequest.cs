using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetTeamMembers;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetTeamMembers
{
    public class WhenIValidateTheRequest 
    {
        private GetTeamMembersRequestValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetTeamMembersRequestValidator();
        }

        [Test]
        public void ThenShouldReturnValidIfRequestIsValid()
        {
            //Act
            var result = _validator.Validate(new GetTeamMembersRequest(123));

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfNegativeAccountIdIsProvided()
        {
            //Act
            var result = _validator.Validate(new GetTeamMembersRequest(-999));

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfAccountIdIsZero()
        {
            //Act
            var result = _validator.Validate(new GetTeamMembersRequest(0));

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}

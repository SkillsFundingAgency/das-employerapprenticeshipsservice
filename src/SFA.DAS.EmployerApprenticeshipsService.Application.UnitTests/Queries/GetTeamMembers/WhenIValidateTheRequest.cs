using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTeamMembers;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTeamMembers
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
            var result = _validator.Validate(new GetTeamMembersRequest { HashedAccountId = "123ABC"});

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfNoAccountIdIsProvided()
        {
            //Act
            var result = _validator.Validate(new GetTeamMembersRequest());

            //Assert
            Assert.IsFalse(result.IsValid());
        }

        [Test]
        public void ThenShouldReturnInvalidIfAccountIdIsEmpty()
        {
            //Act
            var result = _validator.Validate(new GetTeamMembersRequest {HashedAccountId = string.Empty});

            //Assert
            Assert.IsFalse(result.IsValid());
        }
    }
}

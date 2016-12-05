using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RemoveTeamMember;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RemoveTeamMember
{
    public class WhenIValidateTheCommand
    {
        private RemoveTeamMemberCommandValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new RemoveTeamMemberCommandValidator();
        }

        [Test]
        public void ThenTheErrorDictionaryIsPopulatedWhenThereAreFieldErrors()
        {
            //Act
            var actual = _validator.Validate(new RemoveTeamMemberCommand());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("UserId", "No UserId supplied"),actual.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "No HashedAccountId supplied"),actual.ValidationDictionary );
            Assert.Contains(new KeyValuePair<string,string>("ExternalUserId", "No ExternalUserId supplied"),actual.ValidationDictionary );
            

        }
    }
}

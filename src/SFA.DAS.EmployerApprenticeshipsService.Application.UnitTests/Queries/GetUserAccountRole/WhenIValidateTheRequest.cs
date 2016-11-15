using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetUserAccountRole;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetUserAccountRole
{
    public class WhenIValidateTheRequest
    {
        private GetUserAccountRoleValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _validator = new GetUserAccountRoleValidator();
        }

        [Test]
        public void ThenTrueIsReturnedWhenAllFieldsArePopulated()
        {
            //Act
            var actual = _validator.Validate(new GetUserAccountRoleQuery {AccountId = 12587, ExternalUserId = "123"});

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public void ThenFalseIsReturnedWhenTheFieldsArentPopulated()
        {
            //Act
            var actual = _validator.Validate(new GetUserAccountRoleQuery { AccountId = 0, ExternalUserId = string.Empty});

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("AccountId", "AccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("ExternalUserId", "ExternalUserId has not been supplied"), actual.ValidationDictionary);
        }
    }
}

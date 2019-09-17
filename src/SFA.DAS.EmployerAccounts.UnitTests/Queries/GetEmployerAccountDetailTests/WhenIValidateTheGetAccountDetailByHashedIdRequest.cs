using System.Collections.Generic;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAccountDetailTests
{
    public class WhenIValidateTheGetAccountDetailByHashedIdRequest
    {
        private GetEmployerAccountDetailByHashedIdValidator _validator;
       
        private const string ExpectedHashedId = "4567";  

        [SetUp]
        public void Arrange()
        {      
            _validator = new GetEmployerAccountDetailByHashedIdValidator();
        }

        [Test]
        public void ThenTheResultIsValidWhenAllFieldsArePopulatedAndTheUserIsPartOfTheAccount()
        {
            //Act
            var result = _validator.Validate(new GetEmployerAccountDetailByHashedIdQuery { HashedAccountId = ExpectedHashedId });

            //Assert
            Assert.IsTrue(result.IsValid());
            Assert.IsFalse(result.IsUnauthorized);
        }

        [Test]
        public void ThenTheUnauthorizedFlagIsSetWhenTheUserIsNotPartOfTheAccount()
        {
            //Act
            var result = _validator.Validate(new GetEmployerAccountDetailByHashedIdQuery());

            //Assert
            Assert.IsFalse(result.IsUnauthorized);
        }

        [Test]
        public void ThenTheDictionaryIsPopulatedWithValidationErrors()
        {
            //Act
            var result = _validator.Validate(new GetEmployerAccountDetailByHashedIdQuery());

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "HashedAccountId has not been supplied"), result.ValidationDictionary);
        }
    }
}

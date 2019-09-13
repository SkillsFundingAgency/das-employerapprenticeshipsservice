using System;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccount;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetEmployerAccountTests
{
    public class WhenIValidateTheGetAccountByHashedIdRequest
    {
        private GetEmployerAccountByHashedIdValidator _validator;

        private const string ExpectedHashedId = "4567";
        private const string ExpectedUserId = "asdf4660";
        private Mock<IAuthorizationService> _authorizationService;

        [SetUp]
        public void Arrange()
        {
            _authorizationService = new Mock<IAuthorizationService>();

            _authorizationService
                .Setup(
                    m => m.IsAuthorized(EmployerUserRole.Any))
                .Returns(true);

            _validator = new GetEmployerAccountByHashedIdValidator(_authorizationService.Object);
        }

        [Test]
        public async Task ThenTheResultIsValidWhenAllFieldsArePopulatedAndTheUserIsPartOfTheAccount()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountHashedQuery { HashedAccountId = ExpectedHashedId, UserId = ExpectedUserId });

            //Assert
            Assert.IsTrue(result.IsValid());
            Assert.IsFalse(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheUnauthorizedFlagIsSetWhenTheUserIsNotPartOfTheAccount()
        {
            _authorizationService
                .Setup(
                    m => m.IsAuthorized(EmployerUserRole.Any))
                .Returns(false);

            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountHashedQuery { HashedAccountId = ExpectedHashedId, UserId = ExpectedUserId });

            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheDictionaryIsPopulatedWithValidationErrors()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountHashedQuery());

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("UserId", "UserId has not been supplied"), result.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "HashedAccountId has not been supplied"), result.ValidationDictionary);
        }

    }
}

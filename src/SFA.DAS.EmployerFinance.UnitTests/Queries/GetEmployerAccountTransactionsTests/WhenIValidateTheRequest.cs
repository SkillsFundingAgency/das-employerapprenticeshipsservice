using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccountTransactions;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetEmployerAccountTransactionsTests
{
    public class WhenIValidateTheRequest
    {
        private GetEmployerAccountTransactionsValidator _validator;
        private Mock<IAuthorizationService> _authorizationService;

        [SetUp]
        public void Arrange()
        {
            _authorizationService = new Mock<IAuthorizationService>();
            _validator = new GetEmployerAccountTransactionsValidator(_authorizationService.Object);
        }

        [Test]
        public async Task ThenItIsValidIfAllFieldsArePopUlated()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountTransactionsQuery { ExternalUserId = "123", HashedAccountId = "AD1" });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        [Test]
        public async Task ThenTheResultIsMarkedAsUnauthorizedIfTheUserIsNotAMemberOfTheAccount()
        {
            //Arrange
            _authorizationService.Setup(x => x.IsAuthorized(EmployerUserRole.Any)).Returns(false);

            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountTransactionsQuery { ExternalUserId = "123", HashedAccountId = "AD1" });

            //Assert
            Assert.IsTrue(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenTheResultIsMarkedAsAuthorizedIfNoUserHasBeenProvided()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountTransactionsQuery { ExternalUserId = "", HashedAccountId = "AD1" });

            //Assert
            Assert.IsTrue(result.IsValid());
            Assert.IsFalse(result.IsUnauthorized);
        }

        [Test]
        public async Task ThenIfTheFieldsArentPopulatedThenTheResultIsNotValidAndTheErrorDictionaryIsPopulated()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountTransactionsQuery());

            //Assert
            Assert.IsFalse(result.IsValid());
            Assert.Contains(new KeyValuePair<string, string>("HashedAccountId", "HashedAccountId has not been supplied"), result.ValidationDictionary);
        }
    }
}

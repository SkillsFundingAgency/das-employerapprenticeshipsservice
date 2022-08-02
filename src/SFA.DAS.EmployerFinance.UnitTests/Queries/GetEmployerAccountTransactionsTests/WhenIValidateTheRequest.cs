using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccountTransactions;
using System.Collections.Generic;
using System.Threading.Tasks;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EAS.Domain.Data.Repositories;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetEmployerAccountTransactionsTests
{
    public class WhenIValidateTheRequest
    {
        private GetEmployerAccountTransactionsValidator _validator;
        //private Mock<IAuthorizationService> _authorizationService; //TODO : check  _authorizationService to IMembershipRepository
        private Mock<IMembershipRepository> _membershipRepository;

        [SetUp]
        public void Arrange()
        {
            //_authorizationService = new Mock<IAuthorizationService>(); //TODO : check  _authorizationService to IMembershipRepository
            _membershipRepository = new Mock<IMembershipRepository>();
            
            _validator = new GetEmployerAccountTransactionsValidator(_membershipRepository.Object);
        }

        [Test]
        public async Task ThenItIsValidIfAllFieldsArePopUlated()
        {
            //Act
            var result = await _validator.ValidateAsync(new GetEmployerAccountTransactionsQuery { ExternalUserId = "123", HashedAccountId = "AD1" });

            //Assert
            Assert.IsTrue(result.IsValid());
        }

        //TODO : check this test in Accounts side
        //[Test]
        //public async Task ThenTheResultIsMarkedAsUnauthorizedIfTheUserIsNotAMemberOfTheAccount()
        //{
        //    //Arrange
        //    _authorizationService.Setup(x => x.IsAuthorized(EmployerUserRole.Any)).Returns(false);

        //    //Act
        //    var result = await _validator.ValidateAsync(new GetEmployerAccountTransactionsQuery { ExternalUserId = "123", HashedAccountId = "AD1" });

        //    //Assert
        //    Assert.IsTrue(result.IsUnauthorized);
        //}

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

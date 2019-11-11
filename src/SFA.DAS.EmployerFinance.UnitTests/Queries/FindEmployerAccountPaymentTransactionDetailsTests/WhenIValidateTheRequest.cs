using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerFinance.Queries.FindEmployerAccountLevyDeclarationTransactions;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.FindEmployerAccountPaymentTransactionDetailsTests
{
    public class WhenIValidateTheRequest
    {
        private FindEmployerAccountLevyDeclarationTransactionsQueryValidator _validator;
        private Mock<IAuthorizationService> _authorizationService;

        [SetUp]
        public void Arrange()
        {
            _authorizationService = new Mock<IAuthorizationService>();
            _validator = new FindEmployerAccountLevyDeclarationTransactionsQueryValidator(_authorizationService.Object);
        }

        [Test]
        public async Task ThenTrueIsReturnedWhenAllFieldsArePopulatedAndTheMemberIsPartOfTheAccount()
        {
            //Act
            var actual = await _validator.ValidateAsync(new FindEmployerAccountLevyDeclarationTransactionsQuery
                {
                    ExternalUserId = "test",
                    HashedAccountId = "test",
                    FromDate = DateTime.Now.AddDays(-10),
                    ToDate = DateTime.Now.AddDays(-2)
                });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedAndTheValidtionDictionaryIsPopulatedWhenFieldsArentSupplied()
        {
            //Act
            var actual = await _validator.ValidateAsync(new FindEmployerAccountLevyDeclarationTransactionsQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("FromDate", "From date has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("ToDate", "To date has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("HashedAccountId", "HashedAccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("ExternalUserId", "ExternalUserId has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenTheUnauthorizedFlagIsSetWhenTheUserDoesNotValidateAgainstTheAccount()
        {
            //Arrange
            _authorizationService.Setup(x => x.IsAuthorized(EmployerUserRole.Any)).Returns(false);

            //Act
            var actual = await _validator.ValidateAsync(new FindEmployerAccountLevyDeclarationTransactionsQuery
            {
                ExternalUserId = "test",
                HashedAccountId = "test",
                FromDate = DateTime.Now.AddDays(-10),
                ToDate = DateTime.Now.AddDays(-2)
            });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}

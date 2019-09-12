using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.EmployerUserRoles.Options;
using SFA.DAS.Authorization.Services;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Queries.GetAccountFinanceOverview;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetAccountFinanceOverviewTests
{
    public class WhenIValidateTheRequest
    {
        private Mock<IAuthorizationService> _authorizationService;
        private GetAccountFinanceOverviewQueryValidator _validator;

        [SetUp]
        public void Arrange()
        {
            _authorizationService = new Mock<IAuthorizationService>();
            _validator = new GetAccountFinanceOverviewQueryValidator(_authorizationService.Object);
        }

        [Test]
        public async Task ThenTrueIsReturnedWhenAllFieldsArePopulatedAndTheMemberIsPartOfTheAccount()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountFinanceOverviewQuery
                {
                    AccountId = 10
                });

            //Assert
            Assert.IsTrue(actual.IsValid());
        }

        [Test]
        public async Task ThenFalseIsReturnedAndTheValidtionDictionaryIsPopulatedWhenFieldsArentSupplied()
        {
            //Act
            var actual = await _validator.ValidateAsync(new GetAccountFinanceOverviewQuery());

            //Assert
            Assert.IsFalse(actual.IsValid());
            Assert.Contains(new KeyValuePair<string,string>("AccountId", "AccountId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("AccountHashedId", "AccountHashedId has not been supplied"), actual.ValidationDictionary);
            Assert.Contains(new KeyValuePair<string,string>("UserRef", "UserRef has not been supplied"), actual.ValidationDictionary);
        }

        [Test]
        public async Task ThenTheUnauthorizedFlagIsSetWhenTheUserDoesNotValidateAgainstTheAccount()
        {
            //Arrange
            _authorizationService.Setup(x => x.IsAuthorized(EmployerUserRole.Any)).Returns(false);

            //Act
            var actual = await _validator.ValidateAsync(new GetAccountFinanceOverviewQuery
            {
                AccountId = 10
            });

            //Assert
            Assert.IsTrue(actual.IsUnauthorized);
        }
    }
}

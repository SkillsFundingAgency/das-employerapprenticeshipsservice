using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataHelper;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenGetAccountUsersWithKnownIds
    {
        private ApiIntegrationTester _tester;

        [SetUp]
        public void SetUp()
        {
            _tester = new ApiIntegrationTester();
        }

        [TearDown]
        public void TearDown()
        {
            _tester.Dispose();
        }

        [Test]
        public async Task ThenTheStatusShouldBeFound_AndDataShouldContainOnlyTheExpectedUser()
        {
            // Arrange
            const string accountName = "AccountWhenGetLegalEntitiesWithNonExistentKey";
            const string legalEntityName = "LegalEntityWhenGetLegalEntitiesWithNonExistentKey";
            const string payeReference = "PayeWhenGetLegalEntitiesWithNonExistentKey";
            const string userRef = "3256229B-6CA6-41C7-B1D0-A72A75078632";

            var userInput = TestModelBuilder.User.CreateUserInput(userRef);

            string hashedAccountId;
            using (var testEmployerAccountsDbBuilder = _tester.GetTransientInstance<EmployerAccountsDbBuilder>())
            {
                testEmployerAccountsDbBuilder
                    .EnsureUserExists(userInput)
                    .EnsureAccountExists(TestModelBuilder.Account.CreateAccountInput(accountName, payeReference, testEmployerAccountsDbBuilder.Context.ActiveUser.UserId))
                    .WithLegalEntity(TestModelBuilder.LegalEntity.BuildEntityWithAgreementInput(legalEntityName, testEmployerAccountsDbBuilder.Context.ActiveEmployerAccount.AccountId));

                hashedAccountId = testEmployerAccountsDbBuilder.Context.ActiveEmployerAccount.HashedAccountId;
            }

            var callRequirements = new CallRequirements($"api/accounts/{hashedAccountId}/users")
                .ExpectControllerType(typeof(EmployerAccountsController))
                .AllowStatusCodes(HttpStatusCode.OK);
            
            // Act
            var account = await _tester.InvokeGetAsync<ICollection<TeamMemberViewModel>>(callRequirements);

            // Assert
            Assert.IsNotNull(account.Data);
            Assert.AreEqual(1, account.Data.Count);
            Assert.AreEqual(userInput.UserRef.ToLower(),
                account.Data.Last().UserRef.ToLower());
        }
    }
}
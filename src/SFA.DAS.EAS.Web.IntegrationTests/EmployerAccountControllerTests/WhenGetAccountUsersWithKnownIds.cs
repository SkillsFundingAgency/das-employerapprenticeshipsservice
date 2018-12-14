using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.Api.Controllers;
using SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess;


namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenGetAccountUsersWithKnownIds
    {
        private ApiIntegrationTester _tester;

        [SetUp]
        public void SetUp()
        {
            _tester = new ApiIntegrationTester(TestSetupIoC.CreateIoC);
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

            string hashedAccountId = null;
            string userRef = null;

            await _tester.InitialiseData<EmployerAccountsDbBuilder>(async builder =>
            {
                var data = new TestModelBuilder()
                    .WithNewUser()
                    .WithNewAccount(accountName, payeReference)
                    .WithNewLegalEntity(legalEntityName);

                await builder.SetupDataAsync(data);

                hashedAccountId = data.CurrentAccount.AccountOutput.HashedAccountId;
                userRef = data.CurrentUser.UserOutput.UserRef;
            });

            var callRequirements = new CallRequirements($"api/accounts/{hashedAccountId}/users")
                .ExpectControllerType(typeof(EmployerAccountsController))
                .ExpectStatusCodes(HttpStatusCode.OK);
            
            // Act
            var account = await _tester.InvokeGetAsync<ICollection<TeamMemberViewModel>>(callRequirements);

            // Assert
            Assert.IsNotNull(account.Data);
            Assert.AreEqual(1, account.Data.Count);
            Assert.IsTrue(string.Equals(userRef, account.Data.Last().UserRef, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
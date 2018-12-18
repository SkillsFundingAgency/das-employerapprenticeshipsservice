using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ModelBuilders;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Service.EmployerAccounts
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
            string hashedAccountId = null;
            var userRef = Guid.Empty;

            await _tester.InitialiseData<EmployerAccountsDbBuilder>(async builder =>
            {
                var data = new TestModelBuilder()
                    .WithNewUser()
                    .WithNewAccount()
                    .WithNewLegalEntity();

                await builder.SetupDataAsync(data);

                hashedAccountId = data.CurrentAccount.AccountOutput.HashedAccountId;
                userRef = data.CurrentUser.UserOutput.UserRef;
            });

            var callRequirements = new CallRequirements($"api/accounts/{hashedAccountId}/users");
            
            // Act
            var account = await _tester.InvokeGetAsync<ICollection<TeamMemberViewModel>>(callRequirements);

            // Assert

            account.ExpectControllerType(typeof(EmployerAccountsController));
            account.ExpectStatusCodes(HttpStatusCode.OK);
            Assert.IsNotNull(account.Data);
            Assert.AreEqual(1, account.Data.Count);
            Assert.AreEqual(userRef, Guid.Parse(account.Data.Last().UserRef));
        }
    }
}
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.ModelBuilders;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.DataAccess.Dtos;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.LegalEntitiesControllerTests
{
    [TestFixture]
    public class WhenIGetMultipleLegalEntitiesWithKnownIds
    {
        private ApiIntegrationTester _tester;
        private EmployerAccountSetup _employerAccount;

        [SetUp]
        public async Task Setup()
        {
            _tester = new ApiIntegrationTester(TestSetupIoC.CreateIoC);

            // Arrange
            await _tester.InitialiseData<EmployerAccountsDbBuilder>(async builder =>
            {
                var data = new TestModelBuilder()
                    .WithNewUser()
                    .WithNewAccount()
                    .WithNewLegalEntity();

                await builder.SetupDataAsync(data);

                _employerAccount = data.CurrentAccount;
            });
        }

        [TearDown]
        public void TearDown()
        {
            _tester.Dispose();
        }

        [Test]
        public async Task ThenTheStatusShouldBeFound_ByHashedAccountId()
        {
            var callRequirements =
                new CallRequirements($"api/accounts/{_employerAccount.AccountOutput.HashedAccountId}/legalentities");

            // Act
            var account = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert
            Assert.IsNotNull(account.Data);
            Assert.AreEqual(2, account.Data.Count);
            
            var idsFromApi = account.Data.Select(a => long.Parse(a.Id, NumberStyles.None)).ToArray();

            var idsFromDatabase = _employerAccount.LegalEntities
                .Select(le => le.LegalEntityWithAgreementInputOutput.LegalEntityId)
                .Union(new[]{_employerAccount.AccountOutput.LegalEntityId})
                .ToArray();

            Assert.AreEqual(2, idsFromDatabase.Length,
                "Not the correct number of legal entities created for this test");

            CheckThatApiReturnedAllLegalEntitiesInDatabase(idsFromDatabase, idsFromApi);
            CheckThatApiReturnedOnlyLegalEntitiesInTheDatabase(idsFromDatabase, idsFromApi);
        }

        private void CheckThatApiReturnedAllLegalEntitiesInDatabase(long[] databaseIds, long[] apiIds)
        {
            var legalEntitiesInDatabaseButNotApi = databaseIds
                .Where(legalEntityId => !apiIds.Contains(legalEntityId))
                .ToArray();

            Assert.AreEqual(0, legalEntitiesInDatabaseButNotApi.Length, "Expected legal entities not returned by API");
        }

        private void CheckThatApiReturnedOnlyLegalEntitiesInTheDatabase(long[] databaseIds, long[] apiIds)
        {
            var legalEntitiesInApiButNotDatabase =
                apiIds.Where(id => !databaseIds.Contains(id)).ToArray();

            Assert.AreEqual(0, legalEntitiesInApiButNotDatabase.Length, "Unexpected legal entities returned by API");
        }
    }
}
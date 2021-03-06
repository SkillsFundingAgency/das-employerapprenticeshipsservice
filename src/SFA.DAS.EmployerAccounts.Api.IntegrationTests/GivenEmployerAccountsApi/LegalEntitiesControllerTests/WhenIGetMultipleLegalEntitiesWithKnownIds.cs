﻿using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.Helpers;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.ModelBuilders;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataAccess.Dtos;
using SFA.DAS.EmployerAccounts.Api.Types;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.GivenEmployerAccountsApi.LegalEntitiesControllerTests
{
    [ExcludeFromCodeCoverage]
    [TestFixture]
    public class WhenIGetMultipleLegalEntitiesWithKnownIds
    :GivenEmployerAccountsApi
    {
        private EmployerAccountSetup _employerAccount;

        [SetUp]
        public async Task Setup()
        {
            await InitialiseEmployerAccountData(async builder =>
            {
                var data = new TestModelBuilder()
                    .WithNewUser()
                    .WithNewAccount()
                    .WithNewLegalEntity();

                await builder.SetupDataAsync(data);

                _employerAccount = data.CurrentAccount;
            });

            WhenControllerActionIsCalled($"https://localhost:44330/api/accounts/{_employerAccount.AccountOutput.HashedAccountId}/legalentities");
        }

        [Test]
        public async Task ThenTheStatusShouldBeFound_ByHashedAccountId()
        {
            var resources =
                Response
                    .GetContent<ResourceList>();

            // Assert
            Assert.IsNotNull(resources);
            Assert.AreEqual(2, resources.Count);
            
            var idsFromApi = resources.Select(a => long.Parse(a.Id, NumberStyles.None)).ToArray();

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
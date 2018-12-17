﻿using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.DataHelper;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ModelBuilders;
using SFA.DAS.EmployerAccounts.Models;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Service.EmployerAccounts
{
    [TestFixture]
    public class WhenGetLegalEntitiesWithNonExistentKey
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
        public async Task ThenTheStatusShouldBeNotFound()
        {
            // Arrange
            var callRequirements =
                new CallRequirements("api/accounts/ZZZZZZ/legalentities")
                    .AllowStatusCodes(HttpStatusCode.NotFound)
                    .ExpectControllerType(typeof(LegalEntitiesController));

            // Act
            var legalEntities = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert
            Assert.IsNull(legalEntities.Data);
        }

        [Test]
        public async Task ThenTheStatusShouldBeFound()
        {
            // Arrange
            const string accountName = "ACME Fireworks";
            const string legalEntityName = "RoadRunner Pest Control";
            const string payeReference = "Acme PAYE";

            string hashedAccountId = null;
            _tester.InitialiseData<EmployerAccountsDbBuilder>(builder =>
            {
                // TODO: the way ids are propagated is a bit clunky
                builder
                    .EnsureUserExists(TestModelBuilder.User.CreateUserInput())
                    .EnsureAccountExists(TestModelBuilder.Account.CreateAccountInput(accountName, payeReference, builder.Context.ActiveUser.UserId))
                    .WithLegalEntity(TestModelBuilder.LegalEntity.BuildEntityWithAgreementInput(legalEntityName, builder.Context.ActiveEmployerAccount.AccountId));

                hashedAccountId = builder.Context.ActiveEmployerAccount.HashedAccountId;
            });

            var callRequirements =
                new CallRequirements($"api/accounts/{hashedAccountId}/legalentities")
                    .AllowStatusCodes(HttpStatusCode.OK)
                    .ExpectControllerType(typeof(LegalEntitiesController));

            // Act
            var legalEntities = await _tester.InvokeGetAsync<ResourceList>(callRequirements);

            // Assert
            Assert.IsNotNull(legalEntities.Data);
        }
    }
}
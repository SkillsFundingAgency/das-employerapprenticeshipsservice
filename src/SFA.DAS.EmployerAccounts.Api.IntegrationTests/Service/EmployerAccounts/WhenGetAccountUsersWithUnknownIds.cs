﻿using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Controllers;
using SFA.DAS.EmployerAccounts.Api.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EmployerAccounts.Models.AccountTeam;

namespace SFA.DAS.EmployerAccounts.Api.IntegrationTests.Service.EmployerAccounts
{
    [TestFixture]
    public class WhenGetAccountUsersWithUnknownIds
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

        //[Test]
        public async Task ThenTheStatusShouldBeOK_AndDataShouldContainZeroUsers()
        {
            // Arrange
            var callRequirements = new CallRequirements($"api/accounts/MADE*UP*ID/users");

            // Act
            var response = await _tester.InvokeGetAsync<ICollection<TeamMemberViewModel>>(callRequirements);

            // Assert
            response.ExpectControllerType(typeof(EmployerAccountsController));
            response.ExpectStatusCodes(HttpStatusCode.OK);
            Assert.AreEqual(0, response.Data.Count);

            Assert.Pass("Verified we got http status OK");
        }
    }
}
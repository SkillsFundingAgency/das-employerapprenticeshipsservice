using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.Api.Controllers;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
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

        [Test]
        public async Task ThenTheStatusShouldBeOK_AndDataShouldContainZeroUsers()
        {
            // Arrange
            var callRequirements = new CallRequirements($"api/accounts/MADE*UP*ID/users")
                .ExpectControllerType(typeof(EmployerAccountsController))
                .AllowStatusCodes(HttpStatusCode.OK);

            // Act
            var response = await _tester.InvokeGetAsync<ICollection<TeamMemberViewModel>>(callRequirements);

            // Assert
            Assert.AreEqual(0, response.Data.Count);

            Assert.Pass("Verified we got http status OK");
        }

    }
}
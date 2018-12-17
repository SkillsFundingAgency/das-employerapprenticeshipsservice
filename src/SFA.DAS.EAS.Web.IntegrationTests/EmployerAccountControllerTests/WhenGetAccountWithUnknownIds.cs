using System.Net;
using System.Threading.Tasks;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;
using SFA.DAS.EAS.Account.Api.Controllers;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.EmployerAccountControllerTests
{
    [TestFixture]
    public class WhenGetAccountWithUnknownIds
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
        public async Task ThenTheStatusShouldBeNotFound_ById()
        {
            // Arrange
            var callRequirements = new CallRequirements($"api/accounts/internal/-1");

            // Act
            var response = await _tester.InvokeGetAsync<AccountDetailViewModel>(callRequirements);

            // Assert
            response.ExpectControllerType(typeof(EmployerAccountsController));
            response.ExpectStatusCodes(HttpStatusCode.NotFound);
            Assert.Pass("Verified we got http status NotFound");
        }


        [Test]
        public async Task ThenTheStatusShouldBeNotFound_ByHashedId()
        {
            // Arrange
            var callRequirements = new CallRequirements($"api/accounts/MADE*UP*ID");

            // Act
            var response = await _tester.InvokeGetAsync<AccountDetailViewModel>(callRequirements);

            // Assert
            response.ExpectControllerType(typeof(EmployerAccountsController));
            response.ExpectStatusCodes(HttpStatusCode.NotFound);
        }
    }
}
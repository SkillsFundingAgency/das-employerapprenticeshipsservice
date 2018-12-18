using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Account.API.IntegrationTests.TestUtils.ApiTester;

namespace SFA.DAS.EAS.Account.API.IntegrationTests.RedirectTests
{
    public abstract class ApiRedirectTest
    {
        private ApiIntegrationTester _apiTester;
        private CallResponse _actualResponse;
        protected abstract string PathAndQuery { get; }

        [SetUp]
        public async Task Setup()
        {
            _apiTester = new ApiIntegrationTester();
            _actualResponse = await _apiTester.InvokeGetAsync(new CallRequirements(PathAndQuery));
        }

        [Test]
        public void ThenTheStatusCodeShouldBeFound()
        {
            _actualResponse.Response.StatusCode
                .Should().Be(HttpStatusCode.Found);
        }

        [Test]
        public void ThenIShouldBeRedirectedToTheNewApis()
        {
            _actualResponse.Response.Headers.Location.PathAndQuery.Should().Be(PathAndQuery);
        }

    }
}
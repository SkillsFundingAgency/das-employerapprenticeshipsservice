using Moq;
using NUnit.Framework;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public abstract class ApiClientTestBase
    {
        protected AccountApiConfiguration Configuration;
        internal Mock<SecureHttpClient> HttpClient;
        protected AccountApiClient ApiClient;

        protected const long NumericalAccountId = 12345;
        protected const string TextualAccountId = "ABC123";

        public abstract void HttpClientSetup();

        [SetUp]
        public void Arrange()
        {
            Configuration = new AccountApiConfiguration
            {
                ApiBaseUrl = "http://some-url/"
            };
            
            HttpClient = new Mock<SecureHttpClient>();
            HttpClientSetup();

            ApiClient = new AccountApiClient(Configuration, HttpClient.Object);
        }
    }
}

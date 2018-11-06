using Moq;
using NUnit.Framework;

namespace SFA.DAS.EmployerAccounts.Api.Client.UnitTests
{
    public abstract class ApiClientTestBase
    {
        protected EmployerAccountsApiClientConfiguration Configuration;
        internal Mock<SecureHttpClient> HttpClient;
        protected EmployerAccountsApiClient ApiClient;

        protected const long NumericalAccountId = 12345;
        protected const string TextualAccountId = "ABC123";

        public abstract void HttpClientSetup();

        [SetUp]
        public void Arrange()
        {
            Configuration = new EmployerAccountsApiClientConfiguration
            {
                ApiBaseUrl = "http://some-url/"
            };
            
            HttpClient = new Mock<SecureHttpClient>();
            HttpClientSetup();

            ApiClient = new EmployerAccountsApiClient(Configuration, HttpClient.Object);
        }
    }
}

using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Configuration;

namespace SFA.DAS.EmployerFinance.Api.Client.UnitTests
{
    public abstract class ApiClientTestBase
    {
        protected EmployerFinanceApiClientConfiguration Configuration;
        internal Mock<SecureHttpClient> HttpClient;
        protected EmployerFinanceApiClient ApiClient;

        protected const long NumericalAccountId = 12345;
        protected const string TextualAccountId = "ABC123";

        public abstract void HttpClientSetup();

        [SetUp]
        public void Arrange()
        {
            Configuration = new EmployerFinanceApiClientConfiguration()
            {
                ApiBaseUrl = "http://some-url/"
            };

            HttpClient = new Mock<SecureHttpClient>();
            HttpClientSetup();
            
            ApiClient = new EmployerFinanceApiClient(Configuration, HttpClient.Object);
        }
    }
}

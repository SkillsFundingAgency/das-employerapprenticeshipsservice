using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAPayeScheme : ApiClientTestBase
    {
        private PayeSchemeViewModel _expectedPayeScheme;
        private string _uri;

        public override void HttpClientSetup()
        {
            _uri = $"/api/accounts/{TextualAccountId}/payeschemes/ABC%F123";
            var absoluteUri = Configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            _expectedPayeScheme = new PayeSchemeViewModel
            {
                Ref = "ABC/123",
                Name = "Name"
            };

            HttpClient.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_expectedPayeScheme)));
        }

        [Test]
        public async Task ThenThePayeSchemeIsReturned()
        {
            // Act
            var response = await ApiClient.GetResource<PayeSchemeViewModel>(_uri);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsAssignableFrom<PayeSchemeViewModel>(response);
            response.Should().NotBeNull();
            response.ShouldBeEquivalentTo(_expectedPayeScheme);
        }
    }
}

using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingPayeSchemesForAnAccount : ApiClientTestBase
    {
        private string _uri;
        private List<ResourceViewModel> _payeSchemes;

        public override void HttpClientSetup()
        {
            _uri = $"/api/accounts/{TextualAccountId}/payeschemes";
            var absoluteUri = Configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            _payeSchemes = new List<ResourceViewModel>() { new ResourceViewModel { Id = "1", Href = "/api/payeschemes/test1" } };

            HttpClient.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_payeSchemes)));
        }

        [Test]
        public async Task ThenTheAccountDetailsAreReturned()
        {
            // Act
            var response = await ApiClient.GetPayeSchemesConnectedToAccount(TextualAccountId);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsAssignableFrom<ResourceList>(response);
            response.Should().NotBeNull();
            response.ShouldBeEquivalentTo(_payeSchemes);
        }
    }
}

using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Support.Core.Models;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAPayeScheme : ApiClientTestBase
    {
        private PayeSchemeModel? _expectedPayeScheme;
        private string? _uri;

        protected override void HttpClientSetup()
        {
            _uri = $"/api/accounts/{TextualAccountId}/payeschemes/scheme?ref=ABC%F123";
            var absoluteUri = Configuration!.ApiBaseUrl.TrimEnd('/') + _uri;

            _expectedPayeScheme = new PayeSchemeModel
            {
                Ref = "ABC/123",
                Name = "Name"
            };

            HttpClient!.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_expectedPayeScheme)));
        }

        [Test]
        public async Task ThenThePayeSchemeIsReturned()
        {
            // Act
            var response = await ApiClient!.GetResource<PayeSchemeModel>(_uri);

            // Assert
            Assert.That(response, Is.Not.Null);
            Assert.IsAssignableFrom<PayeSchemeModel>(response);
            response.Should().NotBeNull();
            response.Should().BeEquivalentTo(_expectedPayeScheme);
        }
    }
}

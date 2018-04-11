using System;
using System.Threading.Tasks;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Types;

namespace SFA.DAS.EAS.Account.Api.Client.UnitTests.AccountApiClientTests
{
    public class WhenGettingAccountDetails : ApiClientTestBase
    {
        private AccountDetailViewModel _expectedAccount;
        private string _uri;

        public override void HttpClientSetup()
        {
            _uri = "/api/accounts/ABC123";
            var absoluteUri = Configuration.ApiBaseUrl.TrimEnd('/') + _uri;

            _expectedAccount = new AccountDetailViewModel
            {
                AccountId = 123,
                HashedAccountId = "ABC123",
                PublicHashedAccountId = "ABC321",
                DasAccountName = "Account 1",
                DateRegistered = DateTime.Now.AddYears(-1),
                OwnerEmail = "test@email.com",
                LegalEntities = new ResourceList(new[] { new ResourceViewModel { Id = "1", Href = "/api/legalentities/test1" } }),
                PayeSchemes = new ResourceList(new[] { new ResourceViewModel { Id = "1", Href = "/api/payeschemes/test1" } })
            };

            HttpClient.Setup(c => c.GetAsync(absoluteUri)).Returns(Task.FromResult(JsonConvert.SerializeObject(_expectedAccount)));
        }

        [Test]
        public async Task ThenTheAccountDetailsAreReturned()
        {
            // Act
            var response = await ApiClient.GetResource<AccountDetailViewModel>(_uri);

            // Assert
            Assert.IsNotNull(response);
            Assert.IsAssignableFrom<AccountDetailViewModel>(response);
            response.Should().NotBeNull();
            response.ShouldBeEquivalentTo(_expectedAccount);
        }
    }
}

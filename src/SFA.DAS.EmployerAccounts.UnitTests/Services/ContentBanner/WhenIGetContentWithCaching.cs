

using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.ContentBanner
{
    public class WhenIGetContentWithCaching
    {
        private string _contentType;
        private string _clientId;

        private Mock<IClientContentService> MockClientContentService;
        private Mock<ICacheStorageService> MockCacheStorageService;

        private string CacheKey;
        private string ContentFromCache;
        private string ContentFromApi;
        private EmployerAccountsConfiguration EmployerAccountsConfig;

        private IClientContentService ClientContentServiceWithCaching;

        [SetUp]
        public void Arrange()
        {
            _clientId = "eas-fin";
            _contentType = "banner";

            EmployerAccountsConfig = new EmployerAccountsConfiguration()
            {
                ApplicationId = "eas-fin",
                DefaultCacheExpirationInMinutes = 1
            };
            ContentFromCache = "<p> Example content from cache </p>";
            ContentFromApi = "<p> Example content from api </p>";
            CacheKey = EmployerAccountsConfig.ApplicationId + "_banner";

            MockClientContentService = new Mock<IClientContentService>();
            MockCacheStorageService = new Mock<ICacheStorageService>();

            ClientContentServiceWithCaching = new ClientContentServiceWithCaching(MockClientContentService.Object, MockCacheStorageService.Object, EmployerAccountsConfig);
        }

        [Test]
        public async Task AND_ItIsStoredInTheCache_THEN_ReturnContent()
        {
            StoredInCacheSetup();

            var result = await ClientContentServiceWithCaching.Get(_contentType, EmployerAccountsConfig.ApplicationId);

            Assert.AreEqual(ContentFromCache, result);
            MockCacheStorageService.Verify(c => c.TryGet(CacheKey, out ContentFromCache), Times.Once);
        }

        [Test]
        public async Task AND_ItIsStoredInTheCache_THEN_ContentApiIsNotCalled()
        {
            StoredInCacheSetup();

            var result = await ClientContentServiceWithCaching.Get(_contentType, EmployerAccountsConfig.ApplicationId);

            MockClientContentService.Verify(c => c.Get(_contentType, _clientId), Times.Never);
        }

        [Test]
        public async Task AND_ItIsNotStoredInTheCache_THEN_CallFromClient()
        {
            NotStoredInCacheSetup();

            var result = await ClientContentServiceWithCaching.Get(_contentType, EmployerAccountsConfig.ApplicationId);

            MockClientContentService.Verify(c => c.Get(_contentType, _clientId), Times.Once);
            Assert.AreEqual(ContentFromApi, result);
        }
        private void StoredInCacheSetup()
        {
            MockCacheStorageService.Setup(c => c.TryGet(CacheKey, out ContentFromCache)).Returns(true);
            MockClientContentService.Setup(c => c.Get("banner", CacheKey));
        }

        private void NotStoredInCacheSetup()
        {

            MockCacheStorageService.Setup(c => c.TryGet(CacheKey, out ContentFromCache)).Returns(false);
            MockClientContentService.Setup(c => c.Get("banner", EmployerAccountsConfig.ApplicationId))
                .ReturnsAsync(ContentFromApi);
        }
    }
}

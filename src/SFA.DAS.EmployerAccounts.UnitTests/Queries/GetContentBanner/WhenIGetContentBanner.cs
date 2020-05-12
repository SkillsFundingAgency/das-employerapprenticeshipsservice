using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetClientContent;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Testing.AutoFixture;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetContentBanner
{
    public class WhenIGetContentBanner : QueryBaseTest<GetClientContentRequestHandler, GetClientContentRequest, GetClientContentResponse>
    {
        public override GetClientContentRequest Query { get; set; }
        public override GetClientContentRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetClientContentRequest>> RequestValidator { get; set; }

        public Mock<ICacheStorageService> MockCacheStorageService;
        private Mock<IClientContentService> _contentBannerService;
        private string _contentType;
        private string _clientId;
        private Mock<ILog> _logger;
        public string ContentBanner;
        public static EmployerAccountsConfiguration EmployerAccountsConfiguration;
        [SetUp]
        public void Arrange()
        {
            SetUp();
            EmployerAccountsConfiguration = new EmployerAccountsConfiguration()
            {
                ApplicationId = "eas-acc",
                DefaultCacheExpirationInMinutes = 1
            };
            ContentBanner = "<p>find out how you can pause your apprenticeships<p>";
            MockCacheStorageService = new Mock<ICacheStorageService>();
            _contentType = "banner";
            _clientId = "eas-acc";
            _logger = new Mock<ILog>();
            _contentBannerService = new Mock<IClientContentService>();
            _contentBannerService
                .Setup(cbs => cbs.Get(_contentType, _clientId))
                .ReturnsAsync(ContentBanner);

            Query = new GetClientContentRequest
            {
                ContentType = "banner"
            };

            RequestHandler = new GetClientContentRequestHandler(RequestValidator.Object, _logger.Object,
                _contentBannerService.Object, MockCacheStorageService.Object, EmployerAccountsConfiguration);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            await RequestHandler.Handle(Query);

            //Assert
            _contentBannerService.Verify(x => x.Get(_contentType, _clientId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(ContentBanner, response.Content);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Check_Cache_ReturnIfExists(GetClientContentRequest query1, string contentBanner1,
            Mock<ICacheStorageService> cacheStorageService1,
            GetClientContentRequestHandler requestHandler1,
            Mock<IValidator<GetClientContentRequest>> requestValidator1,
            Mock<ILog> logger,
            Mock<IClientContentService> clientMockontentService)
        {
            query1.ContentType = "Banner";
            query1.UseLegacyStyles = false;

            var key = EmployerAccountsConfiguration.ApplicationId;

            requestValidator1.Setup(r => r.Validate(query1)).Returns(new ValidationResult());

            clientMockontentService.Setup(c => c.Get("banner", key));

            requestHandler1 = new GetClientContentRequestHandler(requestValidator1.Object, logger.Object, clientMockontentService.Object,
                cacheStorageService1.Object, EmployerAccountsConfiguration);

            cacheStorageService1.Setup(c => c.TryGet(key, out contentBanner1))
                .Returns(true);
            //Act
            var result = await requestHandler1.Handle(query1);

            //assert
            Assert.AreEqual(result.Content, contentBanner1);
            cacheStorageService1.Verify(x => x.TryGet(key, out contentBanner1), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Check_Cache_ReturnNull_CallFromClient(GetClientContentRequest query1, string contentBanner1,
            Mock<ICacheStorageService> cacheStorageService1,
            GetClientContentRequestHandler requestHandler1,
            Mock<IValidator<GetClientContentRequest>> requestValidator1,
            Mock<ILog> logger,
            Mock<IClientContentService> clientMockContentService)
        {
            //Arrange
            var key = EmployerAccountsConfiguration.ApplicationId;
            query1.ContentType = "Banner";
            query1.UseLegacyStyles = false;

            requestValidator1.Setup(r => r.Validate(query1)).Returns(new ValidationResult());

            string nullCacheString = null;
            cacheStorageService1.Setup(c => c.TryGet(key, out nullCacheString))
                .Returns(false);

            clientMockContentService.Setup(c => c.Get(query1.ContentType, key))
                .ReturnsAsync(contentBanner1);

            requestHandler1 = new GetClientContentRequestHandler(requestValidator1.Object, logger.Object, clientMockContentService.Object,
                cacheStorageService1.Object, EmployerAccountsConfiguration);

            //Act
            var result = await requestHandler1.Handle(query1);

            //assert
            Assert.AreEqual(result.Content, contentBanner1);
        }
    }
}

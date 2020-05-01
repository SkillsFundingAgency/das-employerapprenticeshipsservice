using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
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
        private ContentType _contentType;
        private string _clientId;
        private Mock<ILog> _logger;
        public string ContentBanner;

        [SetUp]
        public void Arrange()
        {
            SetUp();
            ContentBanner = "<p>find out how you can pause your apprenticeships<p>";
            MockCacheStorageService = new Mock<ICacheStorageService>();
            _contentType = ContentType.Banner;
            _clientId = "eas-acc";
            _logger = new Mock<ILog>();
            _contentBannerService = new Mock<IClientContentService>();
            _contentBannerService
                .Setup(cbs => cbs.GetContentByClientId(_contentType, _clientId))
                .ReturnsAsync(ContentBanner);

            Query = new GetClientContentRequest
            {
                ContentType = "banner",
                ClientId = _clientId
            };

            RequestHandler = new GetClientContentRequestHandler(RequestValidator.Object, _logger.Object,
                _contentBannerService.Object, MockCacheStorageService.Object);
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheServiceIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _contentBannerService.Verify(x => x.GetContentByClientId(_contentType, _clientId), Times.Once);
        }

        public override Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            throw new System.NotImplementedException();
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(ContentBanner, response.ContentBanner);
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
            query1.ClientId = "eas-acc";

            requestValidator1.Setup(r => r.Validate(query1)).Returns(new ValidationResult());
            clientMockontentService.Setup(c => c.GetContentByClientId(ContentType.Banner, "eas-acc"));
            requestHandler1 = new GetClientContentRequestHandler(requestValidator1.Object, logger.Object, clientMockontentService.Object,
                cacheStorageService1.Object);

            cacheStorageService1.Setup(c => c.RetrieveFromCache<string>(query1.ClientId))
                .ReturnsAsync(contentBanner1);
            //Act
            var result = await requestHandler1.Handle(query1);

            //assert
            Assert.AreEqual(result.ContentBanner, contentBanner1);
            cacheStorageService1.Verify(x => x.RetrieveFromCache<string>(query1.ClientId), Times.Once);
        }

        [Test, RecursiveMoqAutoData]
        public async Task Check_Cache_ReturnNull_CallFromClient(GetClientContentRequest Query1, string ContentBanner1,
            Mock<ICacheStorageService> cacheStorageService1,
            GetClientContentRequestHandler requestHandler1,
            Mock<IValidator<GetClientContentRequest>> requestValidator1,
            Mock<ILog> logger,
            Mock<IClientContentService> clientMockontentService)
        {
            Query1.ContentType = "Banner";
            Query1.ClientId = "eas-acc";

            requestValidator1.Setup(r => r.Validate(Query1)).Returns(new ValidationResult());
            clientMockontentService.Setup(c => c.GetContentByClientId(ContentType.Banner, "eas-acc"));
            requestHandler1 = new GetClientContentRequestHandler(requestValidator1.Object, logger.Object, clientMockontentService.Object,
                cacheStorageService1.Object);

            clientMockontentService.Setup(c => c.GetContentByClientId(ContentType.Banner, Query1.ClientId))
                .ReturnsAsync(ContentBanner1);
            SetupEmptyCache(Query1, cacheStorageService1);
             //Act
             var result = await requestHandler1.Handle(Query1);

            //assert
            Assert.AreEqual(result.ContentBanner, ContentBanner1);
        }

        private static void SetupEmptyCache(GetClientContentRequest query, Mock<ICacheStorageService> cacheStorageService)
        {
            cacheStorageService
                .Setup(x => x.RetrieveFromCache<string>(query.ClientId))
                .ReturnsAsync((string) null);
        }
    }
}

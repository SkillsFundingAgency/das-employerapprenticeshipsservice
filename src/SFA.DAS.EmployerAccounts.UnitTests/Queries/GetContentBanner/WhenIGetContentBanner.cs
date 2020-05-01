using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetClientContent;
using SFA.DAS.NLog.Logger;
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
            MockCacheStorageService = new Mock<ICacheStorageService>();
            ContentBanner = "<p>find out how you can pause your apprenticeships<p>";
            _contentType = ContentType.Banner;
            _clientId = "eas-acc";
            _logger = new Mock<ILog>();
            _contentBannerService = new Mock<IClientContentService>();
            _contentBannerService
                .Setup(cbs => cbs.GetContentByClientId(_contentType, _clientId))
                .ReturnsAsync(ContentBanner);

            RequestHandler = new GetClientContentRequestHandler(RequestValidator.Object, _logger.Object, 
                _contentBannerService.Object, MockCacheStorageService.Object);

            Query = new GetClientContentRequest
            {
                ContentType = "banner",
                ClientId = _clientId
            };
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
    }
}

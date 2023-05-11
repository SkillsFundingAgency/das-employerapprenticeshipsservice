using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Queries.GetContent;
using SFA.DAS.Testing.AutoFixture;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetContentBanner;

public class WhenIGetContentBanner : QueryBaseTest<GetContentRequestHandler, GetContentRequest, GetContentResponse>
{
    public override GetContentRequest Query { get; set; }
    public override GetContentRequestHandler RequestHandler { get; set; }
    public override Mock<IValidator<GetContentRequest>> RequestValidator { get; set; }

    public Mock<ICacheStorageService> MockCacheStorageService;
    private Mock<IContentApiClient> _contentBannerService;
    private string _contentType;
    private string _clientId;
    private Mock<ILogger<GetContentRequestHandler>> _logger;
    public string ContentBanner;
    public EmployerAccountsConfiguration EmployerAccountsConfiguration;

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
        _logger = new Mock<ILogger<GetContentRequestHandler>>();
        _contentBannerService = new Mock<IContentApiClient>();
        _contentBannerService
            .Setup(cbs => cbs.Get(_contentType, _clientId))
            .ReturnsAsync(ContentBanner);

        Query = new GetContentRequest
        {
            ContentType = "banner"
        };

        RequestHandler = new GetContentRequestHandler(RequestValidator.Object, _logger.Object, _contentBannerService.Object, EmployerAccountsConfiguration);
    }

    [Test]
    public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
    {
        await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        _contentBannerService.Verify(x => x.Get(_contentType, _clientId), Times.Once);
    }

    [Test]
    public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
    {
        //Act
        var response = await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        Assert.AreEqual(ContentBanner, response.Content);
    }

    [Test, RecursiveMoqAutoData]
    public async Task Check_Cache_ReturnNull_CallFromClient(GetContentRequest query1, string contentBanner1,
        Mock<ICacheStorageService> cacheStorageService1,
        GetContentRequestHandler requestHandler1,
        Mock<IValidator<GetContentRequest>> requestValidator1,
        Mock<ILogger<GetContentRequestHandler>> logger,
        Mock<IContentApiClient> MockContentService)
    {
        //Arrange
        var key = EmployerAccountsConfiguration.ApplicationId;
        query1.ContentType = "Banner";
        query1.UseLegacyStyles = false;

        requestValidator1.Setup(r => r.Validate(query1)).Returns(new ValidationResult());

        string nullCacheString = null;
        var cacheKey = key + "_banner";
        cacheStorageService1.Setup(c => c.TryGet(cacheKey, out nullCacheString))
            .Returns(false);

        MockContentService.Setup(c => c.Get(query1.ContentType, key))
            .ReturnsAsync(contentBanner1);

        requestHandler1 = new GetContentRequestHandler(requestValidator1.Object, logger.Object, MockContentService.Object, EmployerAccountsConfiguration);

        //Act
        var result = await requestHandler1.Handle(query1, CancellationToken.None);

        //assert
        Assert.AreEqual(result.Content, contentBanner1);
    }
}
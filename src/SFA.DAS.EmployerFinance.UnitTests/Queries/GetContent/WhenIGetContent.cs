using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Queries.GetContent;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetContent
{
    public class WhenIGetContent : QueryBaseTest<GetContentRequestHandler, GetContentRequest, GetContentResponse>
    {
        public override GetContentRequest Query { get; set; }
        public override GetContentRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetContentRequest>> RequestValidator { get; set; }

        private string _contentType;
        private string _clientId;

        private string CacheKey;
        private string Content;
        public EmployerFinanceConfiguration EmployerFinanceConfiguration;

        private Mock<ILog> MockLogger;
        private Mock<IContentService> MockContentService;
        private Mock<ICacheStorageService> MockCacheStorageService;

        [SetUp]
        public void Arrange()
        {
            SetUp();
            _clientId = "eas-fin";
            _contentType = "banner";

            EmployerFinanceConfiguration = new EmployerFinanceConfiguration()
            {
                ApplicationId = "eas-fin",
                DefaultCacheExpirationInMinutes = 1
            };
            Content = "<p> Example content </p>";
            CacheKey = EmployerFinanceConfiguration.ApplicationId + "_banner";

            MockLogger = new Mock<ILog>();
            MockContentService = new Mock<IContentService>();
            MockCacheStorageService = new Mock<ICacheStorageService>();
            

            MockContentService
                .Setup(cs => cs.Get(_contentType, _clientId))
                .ReturnsAsync(Content);

            Query = new GetContentRequest
            {
                ContentType = "banner"
            };

            RequestHandler = new GetContentRequestHandler(RequestValidator.Object, MockLogger.Object,
                MockContentService.Object, MockCacheStorageService.Object, EmployerFinanceConfiguration);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            NotStoredInCacheSetup();

            await RequestHandler.Handle(Query);

            MockContentService.Verify(x => x.Get(_contentType, _clientId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            NotStoredInCacheSetup();

            await RequestHandler.Handle(Query);

            MockContentService.Verify(x => x.Get(_contentType, _clientId), Times.Once);
        }

        private void NotStoredInCacheSetup()
        {
            MockCacheStorageService.Setup(c => c.TryGet(CacheKey, out Content)).Returns(false);
            MockContentService.Setup(c => c.Get("banner", CacheKey))
                .ReturnsAsync(Content);
        }
    }
}

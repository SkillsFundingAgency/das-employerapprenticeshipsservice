using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Configuration;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Queries.GetClientContent;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetClientContent
{
    public class WhenIGetContent : QueryBaseTest<GetClientContentRequestHandler, GetClientContentRequest, GetClientContentResponse>
    {
        public override GetClientContentRequest Query { get; set; }
        public override GetClientContentRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetClientContentRequest>> RequestValidator { get; set; }

        private string _contentType;
        private string _clientId;

        private string CacheKey;
        private string Content;
        public EmployerFinanceConfiguration EmployerFinanceConfiguration;

        private Mock<ILog> MockLogger;
        private Mock<IClientContentService> MockClientContentService;
        private Mock<ICacheStorageService> MockCacheStorageService;
        private Mock<IValidator<GetClientContentRequest>> MockValidator;

        [SetUp]
        public void Arrange()
        {
            _clientId = "eas-fin";
            _contentType = "banner";

            EmployerFinanceConfiguration = new EmployerFinanceConfiguration()
            {
                ApplicationId = "eas-fin",
                DefaultCacheExpirationInMinutes = 1
            };
            Content = "<p> Example content </p>";
            CacheKey = EmployerFinanceConfiguration.ApplicationId;

            MockLogger = new Mock<ILog>();
            MockClientContentService = new Mock<IClientContentService>();
            MockCacheStorageService = new Mock<ICacheStorageService>();
            MockValidator = new Mock<IValidator<GetClientContentRequest>>();

            MockClientContentService
                .Setup(cs => cs.Get(_contentType, _clientId))
                .ReturnsAsync(Content);

            Query = new GetClientContentRequest
            {
                ContentType = "banner"
            };

            RequestHandler = new GetClientContentRequestHandler(RequestValidator.Object, MockLogger.Object,
                MockClientContentService.Object, MockCacheStorageService.Object, EmployerFinanceConfiguration);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            await RequestHandler.Handle(Query);

            MockClientContentService.Verify(x => x.Get(_contentType, _clientId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            await RequestHandler.Handle(Query);

            MockClientContentService.Verify(x => x.Get(_contentType, _clientId), Times.Once);
        }

        [Test]
        public async Task AND_it_is_stored_in_the_cache_THEN_return_content()
        {
            StoredInCacheSetup();

            var result = await RequestHandler.Handle(Query);

            Assert.AreEqual(Content, result.Content);
            MockCacheStorageService.Verify(c => c.TryGet(CacheKey, out Content), Times.Once);
        }

        [Test]
        public async Task AND_it_is_stored_in_the_cache_THEN_content_api_is_not_called()
        {
            StoredInCacheSetup();

            var result = await RequestHandler.Handle(Query);

            MockClientContentService.Verify(c => c.Get(_contentType, _clientId), Times.Never);
        }

        [Test]
        public async Task AND_it_is_not_stored_in_the_cache_THEN_call_from_client()
        {
            MockValidator.Setup(r => r.Validate(Query)).Returns(new ValidationResult());
            MockCacheStorageService.Setup(c => c.TryGet(CacheKey, out Content)).Returns(false);
            MockClientContentService.Setup(c => c.Get("banner", CacheKey))
                .ReturnsAsync(Content);

            var result = await RequestHandler.Handle(Query);

            Assert.AreEqual(Content, result.Content);
        }

        private void StoredInCacheSetup()
        {
            MockValidator.Setup(r => r.Validate(Query)).Returns(new ValidationResult());
            MockCacheStorageService.Setup(c => c.TryGet(CacheKey, out Content)).Returns(true);
            MockClientContentService.Setup(c => c.Get("banner", CacheKey));
        }
    }
}

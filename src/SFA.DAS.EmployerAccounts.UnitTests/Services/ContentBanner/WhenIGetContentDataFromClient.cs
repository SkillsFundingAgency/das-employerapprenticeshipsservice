using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.Api.Common.Interfaces;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.ContentBanner
{
    class WhenIGetContentDataFromService
    {
        private IContentApiClient _sut;
        private ContentClientApiConfiguration _configuration;
        private string _testData;
        
        private string _apiBaseUrl;        
        private string _identifierUri;
        string applicationId = "eas-acc";
        string type = "banner";
        Mock<HttpMessageHandler> _mockHttpMessageHandler;

        [SetUp]
        public void Arrange()
        {
            _apiBaseUrl = $"http://{Guid.NewGuid().ToString()}/";            
            _identifierUri = Guid.NewGuid().ToString();

           _configuration = new ContentClientApiConfiguration
            {
                ApiBaseUrl = _apiBaseUrl,
                IdentifierUri = _identifierUri                
            };

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            _testData = "<h1>My First Heading</h1>" +
                        "<p>My first paragraph.</p>";

            _mockHttpMessageHandler
                  .Protected()
                  .Setup<Task<HttpResponseMessage>>("SendAsync",
                      ItExpr.IsAny<HttpRequestMessage>(),
                      ItExpr.IsAny<CancellationToken>())
                  .ReturnsAsync(new HttpResponseMessage
                  {
                      Content = new StringContent(_testData),
                      StatusCode = HttpStatusCode.OK
                  }
                  ).Verifiable("");

            var httpClient = new HttpClient(_mockHttpMessageHandler.Object);

            _sut = new ContentApiClient(httpClient, _configuration, Mock.Of<IAzureClientCredentialHelper>());
        }

        [Test]
        public async Task Verify_ClientApiIsCalled()
        {
            //act
            await _sut.Get(type, applicationId);

            //assert
            _mockHttpMessageHandler
                .Protected()
                .Verify("SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                       && r.RequestUri.AbsoluteUri == $"{_configuration.ApiBaseUrl}api/content?applicationId={applicationId}&type={type}"),
            ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task ClientApiIsCalled_ContentIsReturned()
        {
            //act
            var content = await _sut.Get(type, applicationId);

            //assert
            Assert.AreEqual(content,_testData);
        }
    }
}

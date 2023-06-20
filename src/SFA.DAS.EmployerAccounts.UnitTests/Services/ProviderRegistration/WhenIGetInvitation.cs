using System;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.Protected;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Configuration;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Services;

namespace SFA.DAS.EmployerAccounts.UnitTests.Services.ProviderRegistration
{
    class WhenIGetInvitation
    {
        private IProviderRegistrationApiClient _sut;
        private ProviderRegistrationClientApiConfiguration _configuration;
        private string _correlationId;
        private string _testData;
        private string _apiBaseUrl;       
        Mock<HttpMessageHandler> _mockHttpMessageHandler;
        Mock<ILogger<ProviderRegistrationApiClient>> _logger;

        [SetUp]
        public void Arrange()
        {
            _apiBaseUrl = $"http://{Guid.NewGuid().ToString()}/";
            _correlationId = Guid.NewGuid().ToString();
            _testData = "Employer details";

            _logger = new Mock<ILogger<ProviderRegistrationApiClient>>();

            _configuration = new ProviderRegistrationClientApiConfiguration
            {
                BaseUrl = _apiBaseUrl,
                IdentifierUri = string.Empty
            };            

            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();

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

            _sut = new ProviderRegistrationApiClient(httpClient, _configuration, _logger.Object);
        }

        [Test]
        public async Task Then_Verify_ProviderRegistrationApi_ToGetInvitationIsCalled()
        {
            //act
            await _sut.GetInvitations(_correlationId);

            //assert
            _mockHttpMessageHandler
                .Protected()
                .Verify("SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                       && r.RequestUri.AbsoluteUri == $"{_configuration.BaseUrl}api/invitations/{_correlationId}"),
                ItExpr.IsAny<CancellationToken>());
        }

        [Test]
        public async Task WhenUnsubscribeProviderEmail_Then_Verify_ProviderRegistrationApiToUnsubscribeIsCalled()
        {
            //act
            await _sut.Unsubscribe(_correlationId);

            //assert
            _mockHttpMessageHandler
                .Protected()
                .Verify("SendAsync", Times.Once(),
                    ItExpr.Is<HttpRequestMessage>(r => r.Method == HttpMethod.Get
                                                       && r.RequestUri.AbsoluteUri == $"{_configuration.BaseUrl}api/unsubscribe/{_correlationId}"),
                ItExpr.IsAny<CancellationToken>());
        }

    }
}

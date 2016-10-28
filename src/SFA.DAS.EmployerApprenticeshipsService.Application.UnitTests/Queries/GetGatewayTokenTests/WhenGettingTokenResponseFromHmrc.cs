using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetGatewayToken;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.HmrcLevy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetGatewayTokenTests
{
    public class WhenGettingTokenResponseFromHmrc
    {
        private GetGatewayTokenHandler _getGatewayTokenHandler;
        private Mock<IHmrcService> _hmrcService;
        private string ExpectedAccessToken = "kjhg345";

        [SetUp]
        public void Arrange()
        {
            _hmrcService = new Mock<IHmrcService>();
            _hmrcService.Setup(x => x.GetAuthenticationToken(It.IsAny<string>(), It.IsAny<string>())).ReturnsAsync(new HmrcTokenResponse {AccessToken = ExpectedAccessToken});

            _getGatewayTokenHandler = new GetGatewayTokenHandler(_hmrcService.Object);
        }

        [Test]
        public async Task ThenTheHmrcServiceIsCalledWithThePassedAccessCode()
        {
            //Arrange
            var code = "gf56df";
            var url = "http://myurl.local";

            //Act
            await _getGatewayTokenHandler.Handle(new GetGatewayTokenQuery {AccessCode = code, RedirectUrl = url});

            //Assert
            _hmrcService.Verify(x=>x.GetAuthenticationToken(url, code));
        }

        [Test]
        public async Task ThenTheReturnValueIsPopulatedFromTheHmrcService()
        {
            //Act
            var actual = await _getGatewayTokenHandler.Handle(new GetGatewayTokenQuery { AccessCode = "", RedirectUrl = "" });

            //Assert
            Assert.IsAssignableFrom<GetGatewayTokenQueryResponse>(actual);
            Assert.AreEqual(ExpectedAccessToken, actual.HmrcTokenResponse.AccessToken);

        }

    }
}

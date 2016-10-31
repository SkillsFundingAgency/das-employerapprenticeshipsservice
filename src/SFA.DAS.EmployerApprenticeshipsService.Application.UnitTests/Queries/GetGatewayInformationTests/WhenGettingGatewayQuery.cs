using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetGatewayInformation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetGatewayInformationTests
{
    public class WhenGettingGatewayQuery
    {
        private GetGatewayInformationHandler _getGatewayInformationHandler;
        private Mock<IHmrcService> _hmrcService;

        [SetUp]
        public void Arrange()
        {
            _hmrcService = new Mock<IHmrcService>();

            _getGatewayInformationHandler = new GetGatewayInformationHandler(_hmrcService.Object);
        }

        [Test]
        public async Task ThenTheUrlIsRetrievedFromTheServiceAndPopulatesTheResponse()
        {
            //Arrange
            var returnUrl = "someurl";

            //Act
            var actual = await _getGatewayInformationHandler.Handle(new GetGatewayInformationQuery {ReturnUrl = returnUrl});

            //Assert
            _hmrcService.Verify(x=>x.GenerateAuthRedirectUrl(returnUrl),Times.Once);

        }
    }
}

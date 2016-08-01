using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Application.Queries.GetGatewayInformation;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Interfaces;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.UnitTests.Queries.GetGatewayInformationTests
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

using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authorization.EmployerFeatures.Models;
using SFA.DAS.Authorization.Features.Services;
using SFA.DAS.EmployerFinance.Dtos;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccountDetail;
using SFA.DAS.EmployerFinance.Web.Controllers;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransfersPage
    {
        private TransferConnectionsController _controller;
        private Mock<IMediator> _mediatorMock;
        private Mock<IFeatureTogglesService<EmployerFeatureToggle>> _featureToggleService;

        [SetUp]
        public void Arrange()
        {
            _mediatorMock = new Mock<IMediator>();
            _featureToggleService = new Mock<IFeatureTogglesService<EmployerFeatureToggle>>();
            _featureToggleService
                .Setup(m => m.GetFeatureToggle(It.Is<string>(p => p == "TransferConnectionRequests")))
                .Returns((string p) => new EmployerFeatureToggle { Feature = p, IsEnabled = true });

            _controller = new TransferConnectionsController(null, null, _mediatorMock.Object, _featureToggleService.Object);
        }

        [Test]
        public async Task ThenShouldBeRedirectedToAccessDeniedWhenTransferConnectionsRequestIsNoEnabled()
        {
            // Arrange
            _mediatorMock
                .Setup(mock => mock.SendAsync(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>()))
                .ReturnsAsync(new GetEmployerAccountDetailByHashedIdResponse { AccountDetail = new AccountDetailDto() });

            _featureToggleService
                .Setup(m => m.GetFeatureToggle(It.Is<string>(p => p == "TransferConnectionRequests")))
                .Returns((string p) => new EmployerFeatureToggle { Feature = p, IsEnabled = false });

            //Act
            var result = await _controller.Index(new GetEmployerAccountDetailByHashedIdQuery { HashedAccountId = "ACC123" }) as RedirectToRouteResult;

            //Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.RouteValues["action"], Is.EqualTo("Index"));
            Assert.That(result.RouteValues["controller"], Is.EqualTo("AccessDenied"));
        }

        [Test]
        public async Task ThenIShouldBeShownTheTransfersPage()
        {
            // Arrange
            _mediatorMock
                .Setup(mock => mock.SendAsync(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>()))
                .ReturnsAsync(new GetEmployerAccountDetailByHashedIdResponse { AccountDetail = new AccountDetailDto()});

            //Act
            var result = await _controller.Index(new GetEmployerAccountDetailByHashedIdQuery { HashedAccountId = "ACC123" }) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(result.Model, Is.Null);
        }
    }
}
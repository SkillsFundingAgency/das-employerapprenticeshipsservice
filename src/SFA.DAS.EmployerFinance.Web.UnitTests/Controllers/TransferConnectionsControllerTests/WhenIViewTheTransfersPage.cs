using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
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

        [SetUp]
        public void Arrange()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new TransferConnectionsController(null, null, _mediatorMock.Object);
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
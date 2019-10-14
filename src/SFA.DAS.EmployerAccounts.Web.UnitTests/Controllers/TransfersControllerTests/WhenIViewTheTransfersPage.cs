using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAccountDetail;
using SFA.DAS.EmployerAccounts.Web.Controllers;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Controllers.TransfersControllerTests
{
    [TestFixture]
    public class WhenIViewTheTransfersPage
    {
        private TransfersController _controller;
        private Mock<IMediator> _mediatorMock;

        [SetUp]
        public void Arrange()
        {
            _mediatorMock = new Mock<IMediator>();
            _controller = new TransfersController(null, null, _mediatorMock.Object);
        }

        [Test]
        public async Task ThenIShouldBeShownTheTransfersPage()
        {
            // Arrange
            _mediatorMock
                .Setup(mock => mock.SendAsync(It.IsAny<GetEmployerAccountDetailByHashedIdQuery>()))
                .ReturnsAsync(new GetEmployerAccountDetailByHashedIdResponse { Account = new AccountDetail()});

            //Act
            var result = await _controller.Index(new GetEmployerAccountDetailByHashedIdQuery { HashedAccountId = "ACC123" }) as ViewResult;

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.ViewName, Is.EqualTo(""));
            Assert.That(result.Model, Is.Null);
        }
    }
}
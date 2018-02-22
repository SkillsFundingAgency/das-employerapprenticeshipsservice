using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Queries.GetTransferAllowance;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.TransferOrchestratorTests
{
    public class WhenIGetTransferDashboardDetails
    {
        private const string HashedAccountId = "123ABC";
        private const decimal ExpectedTransferBalance = 213.56M;

        private readonly string _externalUserId = Guid.NewGuid().ToString();
        private Mock<IMediator> _mediator;
        private TransferOrchestrator _orchestrator;
        private GetTransferAllowanceResponse _response;
        private Mock<ILog> _logger;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();

            _response = new GetTransferAllowanceResponse { Balance = ExpectedTransferBalance };

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferAllowanceQuery>()))
                .ReturnsAsync(_response);

            _orchestrator = new TransferOrchestrator(_mediator.Object, _logger.Object);
        }

        [Test]
        public async Task ThenIShouldGetCurrentTransferBalance()
        {
            //Act
            var viewModel = await _orchestrator.GetTransferAllowance(HashedAccountId, _externalUserId);

            //Assert
            Assert.IsNotNull(viewModel?.Data);
            Assert.AreEqual(ExpectedTransferBalance, viewModel.Data.TransferAllowance);
        }

        [Test]
        public void ThenIfAnExceptionOccursItWillNoBeSupressed()
        {
            //Assign
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferAllowanceQuery>()))
                .Throws<Exception>();

            //Act + Assert
            Assert.ThrowsAsync<Exception>(() => _orchestrator.GetTransferAllowance(HashedAccountId, _externalUserId));
        }

        [Test]
        public void ThenIfAnErrorOccursItShouldBeLogged()
        {
            //Arrange
            var exception = new Exception();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferAllowanceQuery>()))
                .ThrowsAsync(exception);

            //Act
            Assert.ThrowsAsync<Exception>(() => _orchestrator.GetTransferAllowance(HashedAccountId, _externalUserId));

            //Assert
            _logger.Verify(x => x.Error(exception, It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ThenIfAnInvalidRequestOccursItShouldThrowTheException()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferAllowanceQuery>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string>()));

            //Act + Assert
            Assert.ThrowsAsync<InvalidRequestException>(() =>
                _orchestrator.GetTransferAllowance(HashedAccountId, _externalUserId));
        }
    }
}

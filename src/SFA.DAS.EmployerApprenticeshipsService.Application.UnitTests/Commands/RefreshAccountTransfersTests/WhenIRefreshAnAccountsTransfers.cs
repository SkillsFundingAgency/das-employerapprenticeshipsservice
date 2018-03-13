using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RefreshAccountTransfersTests
{
    public class WhenIRefreshAnAccountsTransfers
    {
        private RefreshAccountTransfersCommandHandler _handler;
        private Mock<IValidator<RefreshAccountTransfersCommand>> _validator;
        private Mock<IPaymentService> _paymentService;
        private Mock<ITransferRepository> _transferRepository;
        private Mock<ILog> _logger;
        private ICollection<AccountTransfer> _transfers;
        private RefreshAccountTransfersCommand _command;


        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<RefreshAccountTransfersCommand>>();
            _paymentService = new Mock<IPaymentService>();
            _transferRepository = new Mock<ITransferRepository>();
            _logger = new Mock<ILog>();

            _transfers = new List<AccountTransfer>
            {
                new AccountTransfer()
            };

            _command = new RefreshAccountTransfersCommand
            {
                AccountId = 123,
                PeriodEnd = "1718-R01"
            };

            _handler = new RefreshAccountTransfersCommandHandler(
                _validator.Object, _paymentService.Object, _transferRepository.Object, _logger.Object);

            _validator.Setup(x => x.Validate(It.IsAny<RefreshAccountTransfersCommand>()))
                .Returns(new ValidationResult());

            _paymentService.Setup(x => x.GetAccountTransfers(It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync(_transfers);
        }

        [Test]
        public async Task ThenTheTransfersShouldBeRetrieved()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _paymentService.Verify(x => x.GetAccountTransfers(_command.PeriodEnd, _command.AccountId), Times.Once);
        }

        [Test]
        public async Task ThenTheRetrievedTransfersShouldBeSaved()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _transferRepository.Verify(x => x.CreateAccountTransfers(_transfers), Times.Once);
        }

        [Test]
        public void ThenIfTheCommandIsNotValidWeShouldBeInformed()
        {
            //Assign
            _validator.Setup(x => x.Validate(It.IsAny<RefreshAccountTransfersCommand>()))
                .Returns(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        {nameof(RefreshAccountTransfersCommand.AccountId), "Error"}
                    }
                });

            //Act + Assert
            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));
        }

        [Test]
        public async Task ThenThePaymentShouldNotBeRetrievedIfTheCommandIsInvalid()
        {
            //Assign
            _validator.Setup(x => x.Validate(It.IsAny<RefreshAccountTransfersCommand>()))
                .Returns(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        {nameof(RefreshAccountTransfersCommand.AccountId), "Error"}
                    }
                });

            //Act
            try
            {
                await _handler.Handle(_command);
            }
            catch (InvalidRequestException)
            {
                //Swallow the invalid request exception for this test as we are expecting one
            }

            //Assert
            _paymentService.Verify(x => x.GetAccountTransfers(_command.PeriodEnd, _command.AccountId), Times.Never);
        }

        [Test]
        public async Task ThenTheTransfersShouldNotBeSavedIfTheCommandIsInvalid()
        {
            //Assign
            _validator.Setup(x => x.Validate(It.IsAny<RefreshAccountTransfersCommand>()))
                .Returns(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        { nameof(RefreshAccountTransfersCommand.AccountId), "Error"}
                    }
                });

            //Act
            try
            {
                await _handler.Handle(_command);
            }
            catch (InvalidRequestException)
            {
                //Swallow the invalid request exception for this test as we are expecting one
            }

            //Assert
            _transferRepository.Verify(x => x.CreateAccountTransfers(_transfers), Times.Never);
        }

        [Test]
        public void ThenIfWeCannotGetTransfersWeShouldNotTryToProcessThem()
        {
            //Assert
            _paymentService.Setup(x => x.GetAccountTransfers(It.IsAny<string>(), It.IsAny<long>()))
                .Throws<WebException>();

            //Act + Assert
            Assert.ThrowsAsync<WebException>(() => _handler.Handle(_command));

            _transferRepository.Verify(x => x.CreateAccountTransfers(_transfers), Times.Never);
        }

        [Test]
        public void ThenIfWeCannotGetTransfersWeShouldLogAnError()
        {
            //Assert
            var exception = new Exception();
            _paymentService.Setup(x => x.GetAccountTransfers(It.IsAny<string>(), It.IsAny<long>()))
                .Throws(exception);

            //Act + Assert
            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_command));

            _logger.Verify(x => x.Error(exception, It.IsAny<string>()), Times.Once);
        }
    }
}

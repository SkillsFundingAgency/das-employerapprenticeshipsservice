using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.RefreshAccountTransfers;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.RefreshAccountTransfersTests
{
    public class WhenIRefreshAnAccountsTransfers
    {
        private const string ReceiverAccountName = "Receiver Account";

        private RefreshAccountTransfersCommandHandler _handler;
        private Mock<IValidator<RefreshAccountTransfersCommand>> _validator;
        private Mock<IPaymentService> _paymentService;
        private Mock<ITransferRepository> _transferRepository;
        private Mock<IAccountRepository> _accountRepository;
        private Mock<ILog> _logger;
        private ICollection<AccountTransfer> _transfers;
        private RefreshAccountTransfersCommand _command;
        private Mock<IMessagePublisher> _messagePublisher;
        private AccountTransferDetails _details;


        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<RefreshAccountTransfersCommand>>();
            _paymentService = new Mock<IPaymentService>();
            _transferRepository = new Mock<ITransferRepository>();
            _accountRepository = new Mock<IAccountRepository>();
            _messagePublisher = new Mock<IMessagePublisher>();
            _logger = new Mock<ILog>();

            _details = new AccountTransferDetails
            {
                CourseName = "Testing Level 2",
                ApprenticeCount = 3,
                PaymentTotal = 1200
            };

            var accountTransfer = new AccountTransfer
            {
                SenderAccountId = 12,
                ReceiverAccountId = 32,
                ApprenticeshipId = 1245,
                Amount = 1200,
                TransferDate = DateTime.Now
            };

            _transfers = new List<AccountTransfer>
            {
                accountTransfer
            };

            _command = new RefreshAccountTransfersCommand
            {
                AccountId = 123,
                PeriodEnd = "1718-R01"
            };

            _handler = new RefreshAccountTransfersCommandHandler(
                _validator.Object,
                _paymentService.Object,
                _transferRepository.Object,
                _accountRepository.Object,
                _messagePublisher.Object,
                _logger.Object);

            _validator.Setup(x => x.Validate(It.IsAny<RefreshAccountTransfersCommand>()))
                .Returns(new ValidationResult());

            _paymentService.Setup(x => x.GetAccountTransfers(It.IsAny<string>(), It.IsAny<long>()))
                .ReturnsAsync(_transfers);

            _transferRepository.Setup(x => x.GetTransferPaymentDetails(It.IsAny<AccountTransfer>()))
                .ReturnsAsync(_details);

            _accountRepository.Setup(x => x.GetAccountNames(It.IsAny<IEnumerable<long>>()))
                .ReturnsAsync(new Dictionary<long, string>
                {
                    {accountTransfer.ReceiverAccountId, ReceiverAccountName}
                });
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
        public async Task ThenTheRetrievedTransfersShouldBeLinkedToPeriodEnd()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _transferRepository.Verify(x => x.CreateAccountTransfers(
                It.Is<IEnumerable<AccountTransfer>>(transfers =>
                   transfers.All(t => _command.PeriodEnd.Equals(t.PeriodEnd)))), Times.Once);
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

        [Test]
        public async Task ThenATransferCreatedMessageShouldBeCreated()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _messagePublisher.Verify(x => x.PublishAsync(It.Is<AccountTransfersCreatedQueueMessage>(
                msg => msg.SenderAccountId.Equals(_command.AccountId) &&
                       msg.PeriodEnd.Equals(_command.PeriodEnd))), Times.Once());
        }

        [Test]

        public async Task ThenPaymentDetailsShouldBeCalledForEachTransfer()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            foreach (var transfer in _transfers)
            {
                _transferRepository.Verify(x => x.GetTransferPaymentDetails(transfer), Times.Once);
            }
        }

        [Test]

        public async Task ThenATransferShouldBeCreatedWithTheCorrectPaymentDetails()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _transferRepository.Verify(x => x.CreateAccountTransfers(It.Is<IEnumerable<AccountTransfer>>(
                transfers =>
                    transfers.All(t => t.CourseName.Equals(_details.CourseName) &&
                                       t.ApprenticeCount.Equals(_details.ApprenticeCount)))), Times.Once);
        }

        [Test]

        public async Task ThenATransferShouldBeCreatedWithTheCorrectReceiverAccountName()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _transferRepository.Verify(x => x.CreateAccountTransfers(It.Is<IEnumerable<AccountTransfer>>(
                transfers =>
                    transfers.All(t => t.ReceiverAccountName.Equals(ReceiverAccountName)))), Times.Once);
        }

        [Test]

        public async Task ThenIfTransferAmountAndPaymentTotalsDoNotMatchAWarningIsLogged()
        {
            //Assign
            _details.PaymentTotal = 10; //Should be 1200

            //Act
            await _handler.Handle(_command);

            //Assert
            _logger.Verify(x => x.Warn("Transfer total does not match transfer payments total"));
        }
    }
}

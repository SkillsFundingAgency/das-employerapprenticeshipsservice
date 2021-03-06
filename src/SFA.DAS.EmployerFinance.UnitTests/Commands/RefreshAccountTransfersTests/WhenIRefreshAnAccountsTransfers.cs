﻿using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.RefreshAccountTransfers;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Models.Transfers;
using SFA.DAS.EmployerFinance.Services;
using SFA.DAS.Messaging.Interfaces;
using SFA.DAS.NLog.Logger;
using SFA.DAS.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.UnitTests.Commands.RefreshAccountTransfersTests
{
    public class WhenIRefreshAnAccountsTransfers
    {
        private const long SenderAccountId = 1;
        private const string SenderAccountName = "Sender Account";
        private const long ReceiverAccountId = 2;
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
        private AccountTransfer _accountTransfer;


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
                CourseName = "Testing",
                CourseLevel = 2,
                ApprenticeCount = 3,
                PaymentTotal = 1200
            };

            _accountTransfer = new AccountTransfer
            {
                SenderAccountId = SenderAccountId,
                SenderAccountName = SenderAccountName,
                ReceiverAccountId = ReceiverAccountId,
                ReceiverAccountName = ReceiverAccountName,
                ApprenticeshipId = 1245,
                Amount = 1200
            };

            _transfers = new List<AccountTransfer>
            {
                _accountTransfer
            };

            _command = new RefreshAccountTransfersCommand
            {
                ReceiverAccountId = ReceiverAccountId,
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

            _accountRepository.Setup(x => x.GetAccountName(ReceiverAccountId))
                .ReturnsAsync(ReceiverAccountName);

            _accountRepository.Setup(x => x.GetAccountNames(It.Is<IEnumerable<long>>(ids => ids.All(id => id == SenderAccountId))))
                .ReturnsAsync(new Dictionary<long, string>
                {
                    {_accountTransfer.SenderAccountId, SenderAccountName}
                });
        }

        [Test]
        public async Task ThenTheTransfersShouldBeRetrieved()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _paymentService.Verify(x => x.GetAccountTransfers(_command.PeriodEnd, _command.ReceiverAccountId), Times.Once);
        }

        [Test]
        public async Task ThenTheRetrievedTransfersShouldBeSaved()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            _transferRepository.Verify(x => x.CreateAccountTransfers(It.Is<IEnumerable<AccountTransfer>>(t =>
                t.All(at => at.ApprenticeshipId.Equals(_accountTransfer.ApprenticeshipId) &&
                            at.SenderAccountId.Equals(_accountTransfer.SenderAccountId) &&
                            at.SenderAccountName.Equals(_accountTransfer.SenderAccountName) &&
                            at.ReceiverAccountId.Equals(_accountTransfer.ReceiverAccountId) &&
                            at.ReceiverAccountName.Equals(_accountTransfer.ReceiverAccountName) &&
                            at.Amount.Equals(_accountTransfer.Amount)))), Times.Once);
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
                        {nameof(RefreshAccountTransfersCommand.ReceiverAccountId), "Error"}
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
                        {nameof(RefreshAccountTransfersCommand.ReceiverAccountId), "Error"}
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
            _paymentService.Verify(x => x.GetAccountTransfers(_command.PeriodEnd, _command.ReceiverAccountId), Times.Never);
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
                        { nameof(RefreshAccountTransfersCommand.ReceiverAccountId), "Error"}
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
            _transferRepository.Verify(x => x.CreateAccountTransfers(It.IsAny<IEnumerable<AccountTransfer>>()), Times.Never);
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

        public async Task ThenPaymentDetailsShouldBeCalledForEachTransfer()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            foreach (var transfer in _transfers)
            {
                _transferRepository.Verify(x => x.GetTransferPaymentDetails(It.Is<AccountTransfer>(t =>
                                t.ApprenticeshipId.Equals(_accountTransfer.ApprenticeshipId) &&
                                t.SenderAccountId.Equals(_accountTransfer.SenderAccountId) &&
                                t.SenderAccountName.Equals(_accountTransfer.SenderAccountName) &&
                                t.ReceiverAccountId.Equals(_accountTransfer.ReceiverAccountId) &&
                                t.ReceiverAccountName.Equals(_accountTransfer.ReceiverAccountName) &&
                                t.Amount.Equals(_accountTransfer.Amount))), Times.Once);
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
                                       t.CourseLevel.Equals(_details.CourseLevel) &&
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

        [Test]
        public async Task ThenATransferPamentsForTheSameApprenticeAndCourseShouldBeAggregated()
        {
            //Assert
            //Duplicate the transfer to simulate two transfers from different delivery periods
            //(We will not be catching duplicate transfers that exactly match as there is no ID or value in the transfer that remains unique to help us)
            _transfers.Add(_accountTransfer);

            _paymentService.Setup(x => x.GetAccountTransfers(_command.PeriodEnd, _command.ReceiverAccountId))
                .ReturnsAsync(_transfers);

            //Act
            await _handler.Handle(_command);

            //Assert
            //There should be only one transfer which has combine amount of above
            _transferRepository.Verify(x => x.CreateAccountTransfers(It.Is<IEnumerable<AccountTransfer>>(
                transfers =>
                    transfers.All(t => t.Amount.Equals(_accountTransfer.Amount * 2)))), Times.Once);
        }

        [Test]
        public async Task ThenATransferPamentsForTheSameApprenticeButDifferentCourseShouldNotBeAggregated()
        {
            //Assert

            _transfers.Add(new AccountTransfer
            {
                Amount = _accountTransfer.Amount,
                PeriodEnd = _accountTransfer.PeriodEnd,
                SenderAccountId = _accountTransfer.SenderAccountId,
                ReceiverAccountId = _accountTransfer.ReceiverAccountId,
                ApprenticeshipId = _accountTransfer.ApprenticeshipId + 1
            });


            _paymentService.Setup(x => x.GetAccountTransfers(_command.PeriodEnd, _command.ReceiverAccountId))
                .ReturnsAsync(_transfers);


            //Act
            await _handler.Handle(_command);

            //Assert
            _transferRepository.Verify(x => x.CreateAccountTransfers(It.Is<IEnumerable<AccountTransfer>>(
                transfers => transfers.Count().Equals(2))), Times.Once);

            _transferRepository.Verify(x => x.CreateAccountTransfers(It.Is<IEnumerable<AccountTransfer>>(
                transfers =>
                    transfers.All(t => t.Amount.Equals(_accountTransfer.Amount)))), Times.Once);
        }
    }
}

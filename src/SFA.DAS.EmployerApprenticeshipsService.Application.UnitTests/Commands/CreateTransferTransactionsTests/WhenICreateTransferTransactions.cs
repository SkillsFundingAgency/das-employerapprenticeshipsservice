using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Commands.CreateTransferTransactions;
using SFA.DAS.EAS.Application.Exceptions;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.NLog.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Commands.CreateTransferTransactionsTests
{
    public class WhenICreateTransferTransactions
    {
        private const string SenderAccountName = "Test Account Sender";
        private const string ReceiverAccountName = "Test Account Receiver";

        private CreateTransferTransactionsCommandHandler _handler;
        private Mock<IValidator<CreateTransferTransactionsCommand>> _validator;
        private Mock<ITransferRepository> _transferRepository;
        private Mock<ITransactionRepository> _transactionRepository;
        private Mock<ILog> _logger;
        private CreateTransferTransactionsCommand _command;
        private List<AccountTransfer> _accountTransfers;

        [SetUp]
        public void Arrange()
        {
            _validator = new Mock<IValidator<CreateTransferTransactionsCommand>>();
            _transferRepository = new Mock<ITransferRepository>();
            _transactionRepository = new Mock<ITransactionRepository>();
            _logger = new Mock<ILog>();

            _accountTransfers = new List<AccountTransfer>
            {
                new AccountTransfer
                {
                    SenderAccountId = 1,
                    SenderAccountName = SenderAccountName,
                    ReceiverAccountId = 2,
                    ReceiverAccountName = ReceiverAccountName,
                    ApprenticeshipId = 100,
                    Amount = 200
                }
            };

            _command = new CreateTransferTransactionsCommand
            {
                ReceiverAccountId = 123,
                PeriodEnd = "1718-R01"
            };

            _handler = new CreateTransferTransactionsCommandHandler(
                _validator.Object,
                _transferRepository.Object,
                _transactionRepository.Object,
                _logger.Object);

            _validator.Setup(x => x.Validate(It.IsAny<CreateTransferTransactionsCommand>()))
                .Returns(new ValidationResult());

            _transferRepository.Setup(x => x.GetReceiverAccountTransfersByPeriodEnd(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(_accountTransfers);
        }

        [Test]
        public async Task ThenTransferSenderTransactionsShouldBeSaved()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            var transfer = _accountTransfers.First();

            _transactionRepository.Verify(x => x.CreateTransferTransactions(
                It.Is<IEnumerable<TransferTransactionLine>>(transactions =>
                    transactions.First().AccountId.Equals(transfer.SenderAccountId))), Times.Once);

            _transactionRepository.Verify(x => x.CreateTransferTransactions(
                It.Is<IEnumerable<TransferTransactionLine>>(transactions =>
                    transactions.First().Amount.Equals(-transfer.Amount))), Times.Once);

            _transactionRepository.Verify(x => x.CreateTransferTransactions(
                It.Is<IEnumerable<TransferTransactionLine>>(transactions =>
                    transactions.First().ReceiverAccountId.Equals(transfer.ReceiverAccountId))), Times.Once);

            _transactionRepository.Verify(x => x.CreateTransferTransactions(
                It.Is<IEnumerable<TransferTransactionLine>>(transactions =>
                    transactions.First().ReceiverAccountName.Equals(ReceiverAccountName))), Times.Once);
        }

        [Test]
        public async Task ThenTransferReceiverTransactionsShouldBeSaved()
        {
            //Act
            await _handler.Handle(_command);

            //Assert
            var transfer = _accountTransfers.First();

            _transactionRepository.Verify(x => x.CreateTransferTransactions(
                It.Is<IEnumerable<TransferTransactionLine>>(transactions =>
                    transactions.ElementAt(1).AccountId.Equals(transfer.ReceiverAccountId))), Times.Once);

            _transactionRepository.Verify(x => x.CreateTransferTransactions(
                It.Is<IEnumerable<TransferTransactionLine>>(transactions =>
                    transactions.ElementAt(1).Amount.Equals(transfer.Amount))), Times.Once);

            _transactionRepository.Verify(x => x.CreateTransferTransactions(
                It.Is<IEnumerable<TransferTransactionLine>>(transactions =>
                    transactions.ElementAt(1).SenderAccountId.Equals(transfer.SenderAccountId))), Times.Once);

            _transactionRepository.Verify(x => x.CreateTransferTransactions(
                It.Is<IEnumerable<TransferTransactionLine>>(transactions =>
                    transactions.ElementAt(1).SenderAccountName.Equals(SenderAccountName))), Times.Once);
        }

        [Test]
        public void ThenIfTheCommandIsNotValidWeShouldBeInformed()
        {
            //Assign
            _validator.Setup(x => x.Validate(It.IsAny<CreateTransferTransactionsCommand>()))
                .Returns(new ValidationResult
                {
                    ValidationDictionary = new Dictionary<string, string>
                    {
                        {nameof(CreateTransferTransactionsCommand.ReceiverAccountId), "Error"}
                    }
                });

            //Act + Assert
            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(_command));
        }

        [Test]
        public async Task ThenSenderTransactionsShouldBeTheSumOfTransfersByReceiver()
        {
            //Arrange
            _accountTransfers.Add(new AccountTransfer
            {
                SenderAccountId = 1,
                SenderAccountName = SenderAccountName,
                ReceiverAccountId = 2,
                ReceiverAccountName = ReceiverAccountName,
                ApprenticeshipId = 200,
                Amount = 100
            });

            var expectedTotalAmount = -_accountTransfers.Sum(t => t.Amount);

            //Act
            await _handler.Handle(_command);

            //Assert


            var transfer = _accountTransfers.First();
            _transactionRepository.Verify(x => x.CreateTransferTransactions(It.Is<IEnumerable<TransferTransactionLine>>(transactions =>
                transactions.First().AccountId.Equals(transfer.SenderAccountId) &&
                transactions.First().Amount.Equals(expectedTotalAmount) &&
                transactions.First().ReceiverAccountId.Equals(transfer.ReceiverAccountId))), Times.Once);
        }

        [Test]
        public async Task ThenReceiverTransactionsShouldBeTheSumOfTransfersByReceiver()
        {
            //Arrange
            _accountTransfers.Add(new AccountTransfer
            {
                SenderAccountId = 1,
                SenderAccountName = SenderAccountName,
                ReceiverAccountId = 2,
                ReceiverAccountName = ReceiverAccountName,
                ApprenticeshipId = 200,
                Amount = 100
            });

            var expectedTotalAmount = _accountTransfers.Sum(t => t.Amount);

            //Act
            await _handler.Handle(_command);

            //Assert


            var transfer = _accountTransfers.First();
            _transactionRepository.Verify(x => x.CreateTransferTransactions(It.Is<IEnumerable<TransferTransactionLine>>(transactions =>
                transactions.ElementAt(1).AccountId.Equals(transfer.ReceiverAccountId) &&
                transactions.ElementAt(1).Amount.Equals(expectedTotalAmount) &&
                transactions.ElementAt(1).SenderAccountId.Equals(transfer.SenderAccountId))), Times.Once);
        }


        [Test]
        public async Task ThenSenderTransactionsShouldBeGroupedByReceiver()
        {
            //Arrange
            _accountTransfers = new List<AccountTransfer>
            {
                new AccountTransfer { SenderAccountId = 1, ReceiverAccountId = 3, ReceiverAccountName = ReceiverAccountName,
                                      ApprenticeshipId = 100, Amount = 100 },

                new AccountTransfer { SenderAccountId = 1, ReceiverAccountId = 3, ReceiverAccountName = ReceiverAccountName,
                                      ApprenticeshipId = 200, Amount = 200 },

                new AccountTransfer { SenderAccountId = 2, ReceiverAccountId = 3, ReceiverAccountName = ReceiverAccountName,
                                      ApprenticeshipId = 300, Amount = 400 },

                new AccountTransfer { SenderAccountId = 2, ReceiverAccountId = 3, ReceiverAccountName = ReceiverAccountName,
                                      ApprenticeshipId = 400, Amount = 800 }
            };

            _transferRepository.Setup(x => x.GetReceiverAccountTransfersByPeriodEnd(It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(_accountTransfers);

            //Act
            await _handler.Handle(_command);

            //Assert
            _transactionRepository.Verify(x => x.CreateTransferTransactions(It.Is<IEnumerable<TransferTransactionLine>>(transactions =>
                transactions.Count(t => t.AccountId.Equals(3)) == 2 &&
                transactions.Any(t => t.Amount.Equals(-300)) &&
                transactions.Any(t => t.Amount.Equals(-1200)))), Times.Once);
        }

        [Test]
        public void ThenIfAnErrorOccursItShouldBeLogged()
        {
            //Arrange
            var expectedException = new Exception();
            _transactionRepository.Setup(x => x.CreateTransferTransactions(It.IsAny<IEnumerable<TransferTransactionLine>>()))
                                  .Throws(expectedException);

            //Act + Assert
            Assert.ThrowsAsync<Exception>(() => _handler.Handle(_command));
            _logger.Verify(x => x.Error(expectedException, It.IsAny<string>()), Times.Once);
        }
    }
}

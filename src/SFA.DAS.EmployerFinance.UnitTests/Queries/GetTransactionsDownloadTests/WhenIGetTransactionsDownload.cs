using System;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerFinance.Data;
using SFA.DAS.EmployerFinance.Formatters.TransactionDowloads;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccount;
using SFA.DAS.EmployerFinance.Queries.GetTransactionsDownload;
using SFA.DAS.Validation;
using MonthYear = SFA.DAS.EmployerFinance.Messages.MonthYear;

namespace SFA.DAS.EmployerFinance.UnitTests.Queries.GetTransactionsDownloadTests
{
    [TestFixture]
    public class WhenIGetTransactionsDownload
    {
        private const long AccountId = 111111;
        private static readonly MonthYear StartDate = new MonthYear { Month = "1", Year = "2000" };
        private static readonly MonthYear EndDate = new MonthYear { Month = "1", Year = "2000" };
        private static readonly DateTime ToDate = new DateTime(2000, 2, 1);

        private GetTransactionsDownloadQueryHandler _handler;
        private GetTransactionsDownloadQuery _query;
        private Mock<ITransactionFormatterFactory> _transactionFormatterFactory;
        private Mock<ITransactionRepository> _transactionsRepository;
        private Mock<IMediator> _mediatorMock;

        [SetUp]
        public void SetUp()
        {
            _transactionsRepository = new Mock<ITransactionRepository>();
            _transactionFormatterFactory = new Mock<ITransactionFormatterFactory>();
            _mediatorMock = new Mock<IMediator>();

            _transactionFormatterFactory
                .Setup(x => x.GetTransactionsFormatterByType(It.IsAny<DownloadFormatType>(), It.IsAny<ApprenticeshipEmployerType>()))
                .Returns(new LevyCsvTransactionFormatter());

            _transactionsRepository.Setup(x => x.GetAllTransactionDetailsForAccountByDate(AccountId, StartDate.ToDate(), ToDate))
                .ReturnsAsync(new TransactionDownloadLine[] { new TransactionDownloadLine() });

            _mediatorMock
                .Setup(mock => mock.SendAsync(It.IsAny<GetEmployerAccountHashedQuery>()))
                .ReturnsAsync(new GetEmployerAccountResponse
                {
                    Account = new Account
                    {
                        ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy
                    }
                }); ;

            _handler = new GetTransactionsDownloadQueryHandler(_transactionFormatterFactory.Object, _transactionsRepository.Object, _mediatorMock.Object);

            _query = new GetTransactionsDownloadQuery
            {
                AccountId = AccountId,
                DownloadFormat = DownloadFormatType.CSV,
                StartDate = StartDate,
                EndDate = EndDate
            };
        }

        [Test]
        public async Task ThenShouldGetATransactionsFormatter()
        {
            await _handler.Handle(_query);

            _transactionFormatterFactory.Verify(x => x.GetTransactionsFormatterByType(_query.DownloadFormat.Value, ApprenticeshipEmployerType.Levy), Times.Once());
        }

        [Test]
        public async Task ThenShouldReturnValidResponse()
        {
            var response = await _handler.Handle(_query);

            Assert.IsNotNull(response);
            Assert.AreEqual("csv", response.FileExtension);
            Assert.AreEqual("text/csv", response.MimeType);
            Assert.IsNotNull(response.FileData);
        }

        [Test]
        public void TheShouldThrowValidationExceptionIfNoTransactionFound()
        {
            _transactionsRepository.Setup(r => r.GetAllTransactionDetailsForAccountByDate(AccountId, StartDate, ToDate))
                .ReturnsAsync(new TransactionDownloadLine[0]);

            Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(_query), "There are no transactions in the date range");
        }
    }
}
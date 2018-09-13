using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownload;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.Validation;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransactionsDownloadTests
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
        
        [SetUp]
        public void SetUp()
        {
            _transactionsRepository = new Mock<ITransactionRepository>();
            _transactionFormatterFactory = new Mock<ITransactionFormatterFactory>();
            
            _transactionFormatterFactory.Setup(x => x.GetTransactionsFormatterByType(It.IsAny<DownloadFormatType>())).Returns(new CsvTransactionFormatter());

            _transactionsRepository.Setup(x => x.GetAllTransactionDetailsForAccountByDate(AccountId, StartDate.ToDate(), ToDate))
                .ReturnsAsync(new List<TransactionDownloadLine> { new TransactionDownloadLine() });

            _handler = new GetTransactionsDownloadQueryHandler(_transactionFormatterFactory.Object, _transactionsRepository.Object);

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
            
            _transactionFormatterFactory.Verify(x => x.GetTransactionsFormatterByType(_query.DownloadFormat.Value), Times.Once());
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
                .ReturnsAsync(new List<TransactionDownloadLine>());

            Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(_query), "There are no transactions in the date range");
        }
    }
}
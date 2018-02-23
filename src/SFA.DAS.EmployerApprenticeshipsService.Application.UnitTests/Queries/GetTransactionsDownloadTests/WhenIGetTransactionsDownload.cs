using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownload;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Domain.Models.UserProfile;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransactionsDownloadTests
{
    public class WhenIGetTransactionsDownload
    {
        private const string HashedAccountId = "ABC123";
        private const string ExternalUserId = "ABCDEF";
        private const long AccountId = 111111;
        private const long UserId = 222222;
        private static readonly MonthYear StartDate = new MonthYear { Month = "1", Year = "2000" };
        private static readonly MonthYear EndDate = new MonthYear { Month = "1", Year = "2000" };
        private static readonly DateTime ToDate = new DateTime(2000, 2, 1);

        private GetTransactionsDownloadQueryHandler _handler;
        private GetTransactionsDownloadQuery _query;
        private GetTransactionsDownloadResponse _response;
        private readonly CurrentUser _currentUser = new CurrentUser { ExternalUserId = ExternalUserId };
        private readonly Mock<IHashingService> _hashingService = new Mock<IHashingService>();
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<ITransactionFormatterFactory> _transactionFormatterFactory;
        private Mock<ITransactionRepository> _transactionsRepository;
        private readonly MembershipView _membershipView = new MembershipView { UserId = UserId };
        
        [SetUp]
        public void SetUp()
        {
            _membershipRepository = new Mock<IMembershipRepository>();
            _transactionsRepository = new Mock<ITransactionRepository>();
            _transactionFormatterFactory = new Mock<ITransactionFormatterFactory>();

            _membershipRepository.Setup(x => x.GetCaller(HashedAccountId, ExternalUserId)).ReturnsAsync(_membershipView);
            _hashingService.Setup(x => x.DecodeValue(HashedAccountId)).Returns(AccountId);
            _transactionFormatterFactory.Setup(x => x.GetTransactionsFormatterByType(It.IsAny<DownloadFormatType>())).Returns(new CsvTransactionFormatter());

            _transactionsRepository.Setup(x => x.GetAllTransactionDetailsForAccountByDate(AccountId, StartDate.ToDate(), ToDate))
                .ReturnsAsync(new List<TransactionDownloadLine> { new TransactionDownloadLine() });

            _handler = new GetTransactionsDownloadQueryHandler(
                _currentUser,
                _hashingService.Object,
                _membershipRepository.Object,
                _transactionFormatterFactory.Object,
                _transactionsRepository.Object);

            _query = new GetTransactionsDownloadQuery
            {
                HashedAccountId = HashedAccountId,
                DownloadFormat = DownloadFormatType.CSV,
                StartDate = StartDate,
                EndDate = EndDate
            };
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            await _handler.Handle(_query);

            _membershipRepository.Verify(r => r.GetCaller(HashedAccountId, ExternalUserId), Times.Once);
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
        public void ThenShouldThrowUnauthorizedAccessExceptionIfUserIsNull()
        {
            _membershipRepository.Setup(r => r.GetCaller(HashedAccountId, ExternalUserId)).ReturnsAsync(null);

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await _handler.Handle(_query));
        }

        [Test]
        public void TheShouldThrowValidationExceptionIfNoTransactionFound()
        {
            _transactionsRepository.Setup(x => x.GetAllTransactionDetailsForAccountByDate(AccountId, StartDate, ToDate))
                .ReturnsAsync(new List<TransactionDownloadLine>());

            Assert.ThrowsAsync<ValidationException>(async () => await _handler.Handle(_query), "There are no transactions in the date range");
        }
    }
}
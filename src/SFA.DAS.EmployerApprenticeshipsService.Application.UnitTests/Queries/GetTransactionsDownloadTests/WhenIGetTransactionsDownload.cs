using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
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
        private const long UserId = 222222;
        private const long FailingUserId = 333333;


        private readonly TransactionsDownloadStartDateMonthYearDateTime _fromDate =
            new TransactionsDownloadStartDateMonthYearDateTime
            {
                Month = 1,
                Year = 2000,
            };

        private readonly TransactionsDownloadEndDateMonthYearDateTime _toDate =
            new TransactionsDownloadEndDateMonthYearDateTime
            {
                Month = 1,
                Year = 2000,
            };

        private readonly TransactionsDownloadStartDateMonthYearDateTime _fromDateNoResults =
            new TransactionsDownloadStartDateMonthYearDateTime
            {
                Month = 1,
                Year = 2000,
            };

        private readonly TransactionsDownloadEndDateMonthYearDateTime _toDateNoResults =
            new TransactionsDownloadEndDateMonthYearDateTime
            {
                Month = 1,
                Year = 2000,
            };

        private GetTransactionsDownloadQueryHandler _handler;
        private GetTransactionsDownloadQuery _query;
        private GetTransactionsDownloadResponse _response;
        private readonly CurrentUser _currentUser = new CurrentUser {ExternalUserId = UserId.ToString()};
        private readonly CurrentUser _currentUserFailing = new CurrentUser {ExternalUserId = FailingUserId.ToString()};
        private readonly Mock<IHashingService> _hashingService = new Mock<IHashingService>();
        private Mock<IMembershipRepository> _membershipRepository;
        private Mock<ITransactionRepository> _transactionsRepository;
        private Mock<ITransactionFormatterFactory> _transactionFormatterFactory;

        private readonly MembershipView _membershipView = new MembershipView {UserId = UserId};

        [SetUp]
        public void SetUp()
        {
            _membershipRepository = new Mock<IMembershipRepository>();

            _hashingService.Setup(h => h.DecodeValue(HashedAccountId)).Returns(UserId);
            _membershipRepository.Setup(r => r.GetCaller(HashedAccountId, UserId.ToString()))
                .ReturnsAsync(_membershipView);
            _membershipRepository.Setup(r => r.GetCaller(HashedAccountId, FailingUserId.ToString())).ReturnsAsync(null);

            _transactionsRepository = new Mock<ITransactionRepository>();
            _transactionFormatterFactory = new Mock<ITransactionFormatterFactory>();

            _transactionFormatterFactory.Setup(tf => tf.GetTransactionsFormatterByType(It.IsAny<DownloadFormatType>()))
                .Returns(new CsvTransactionFormatter());

            _transactionsRepository
                .Setup(x => x.GetAllTransactionDetailsForAccountByDate(UserId,
                    _fromDate.ToDateTime(), _toDate.ToDateTime()))
                .ReturnsAsync(new List<TransactionDownloadLine>
                {
                    new TransactionDownloadLine()
                });

            _transactionsRepository
                .Setup(x => x.GetAllTransactionDetailsForAccountByDate(UserId,
                    _fromDateNoResults.ToDateTime(), _toDateNoResults.ToDateTime()))
                .ReturnsAsync(new List<TransactionDownloadLine>());

            _handler = new GetTransactionsDownloadQueryHandler(
                _currentUser,
                _transactionsRepository.Object,
                _transactionFormatterFactory.Object,
                _membershipRepository.Object,
                _hashingService.Object
            );


            _query = new GetTransactionsDownloadQuery
            {
                AccountHashedId = HashedAccountId,
                DownloadFormat = DownloadFormatType.CSV,
                StartDate = _fromDate,
                EndDate = _toDate,
            };
        }

        [Test]
        public async Task ThenShouldGetUser()
        {
            _response = await _handler.Handle(_query);

            _membershipRepository.Verify(r => r.GetCaller(HashedAccountId, UserId.ToString()), Times.Once);
        }

        [Test]
        public void AndGetUserReturnsNullIThrowUnauthorizedException()
        {
            var localHandler = new GetTransactionsDownloadQueryHandler(
                _currentUserFailing,
                _transactionsRepository.Object,
                _transactionFormatterFactory.Object,
                _membershipRepository.Object,
                _hashingService.Object
            );

            var invalidQuery = new GetTransactionsDownloadQuery
            {
                AccountHashedId = HashedAccountId,
                DownloadFormat = DownloadFormatType.CSV,
                StartDate = _fromDate,
                EndDate = _toDate,
            };

            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await localHandler.Handle(invalidQuery));
        }

        [Test]
        public async Task WithStartDateInFutureThenShouldGetValidationErrors()
        {
            var futureDate = DateTime.Today.AddMonths(1);
            var query = new GetTransactionsDownloadQuery
            {
                AccountHashedId = HashedAccountId,
                DownloadFormat = DownloadFormatType.CSV,
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                {
                    Month = futureDate.Month,
                    Year = futureDate.Year
                },
                EndDate = _toDate,
            };

            _response = await _handler.Handle(query);

            Assert.IsTrue(_response.ValidationResult.ValidationDictionary.Any());
            Assert.IsTrue(_response.ValidationResult.ValidationDictionary.Any(v => v.Key == "StartDate"));

            _transactionsRepository.Verify(
                r => r.GetAllTransactionDetailsForAccountByDate(It.IsAny<long>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        public async Task WithEndDateInFutureThenShouldGetValidationErrors()
        {
            var futureDate = DateTime.Today.AddMonths(1);
            var query = new GetTransactionsDownloadQuery
            {
                AccountHashedId = HashedAccountId,
                DownloadFormat = DownloadFormatType.CSV,
                StartDate = _fromDate,
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                {
                    Month = futureDate.Month,
                    Year = futureDate.Year
                }
            };

            _response = await _handler.Handle(query);

            Assert.IsTrue(_response.ValidationResult.ValidationDictionary.Any());
            Assert.IsTrue(_response.ValidationResult.ValidationDictionary.Any(v => v.Key == "EndDate"));

            _transactionsRepository.Verify(
                r => r.GetAllTransactionDetailsForAccountByDate(It.IsAny<long>(), It.IsAny<DateTime>(),
                    It.IsAny<DateTime>()), Times.Never);
        }

        [Test]
        public async Task AndNoTransactionsFoundThenIShouldGetValidationError()
        {
            _transactionsRepository.Setup(x =>
                x.GetAllTransactionDetailsForAccountByDate(UserId, _fromDate.ToDateTime(),
                    _toDate.ToDateTime())).ReturnsAsync(new List<TransactionDownloadLine>());

            _response = await _handler.Handle(_query);

            Assert.IsTrue(_response.ValidationResult.ValidationDictionary.Any());
        }

        [Test]
        public async Task AndTransactionsFoundThenIShouldGetAFormatter()
        {
            _transactionsRepository.Setup(x =>
                    x.GetAllTransactionDetailsForAccountByDate(UserId, _fromDate.ToDateTime(),
                        _toDate.ToDateTime()))
                .ReturnsAsync(new List<TransactionDownloadLine>() {new TransactionDownloadLine()});

            _response = await _handler.Handle(_query);

            Assert.IsFalse(_response.ValidationResult.ValidationDictionary.Any());
            _transactionFormatterFactory.Verify(
                r => r.GetTransactionsFormatterByType(It.IsAny<DownloadFormatType>()), Times.Once());

        }

    }
}
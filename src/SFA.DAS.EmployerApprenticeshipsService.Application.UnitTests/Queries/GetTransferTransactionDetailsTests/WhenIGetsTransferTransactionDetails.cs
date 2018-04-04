using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Application.Queries.GetTransferSenderTransactionDetails;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferTransactionDetailsTests
{
    class WhenIGetsTransferTransactionDetails
    {
        private const long SenderAccountId = 1;
        private const string SenderAccountName = "Test Sender";
        private const string SenderPublicHashedId = "ABC123";

        private const long ReceiverAccountId = 2;
        private const string ReceiverAccountName = "Test Receiver";
        private const string ReceiverPublicHashedId = "DEF456";

        private const long UserId = 45;
        private const string PeriodEnd = "1718-R01";

        private const string FirstCourseName = "Course 1";
        private const string SecondCourseName = "Course 2";

        private GetTransferSenderTransactionDetailsQueryHandler _handler;
        private GetTransferTransactionDetailsQuery _query;
        private Mock<EmployerFinancialDbContext> _db;
        private List<AccountTransfer> _transfers;
        private Mock<IPublicHashingService> _publicHashingService;

        [SetUp]
        public void Assign()
        {
            _db = new Mock<EmployerFinancialDbContext>();

            _publicHashingService = new Mock<IPublicHashingService>();

            _handler = new GetTransferSenderTransactionDetailsQueryHandler(_db.Object, _publicHashingService.Object);

            _transfers = new List<AccountTransfer>
                {
                    new AccountTransfer
                    {
                        SenderAccountId = SenderAccountId,
                        SenderAccountName = SenderAccountName,
                        ReceiverAccountId = ReceiverAccountId,
                        ReceiverAccountName = ReceiverAccountName,
                        ApprenticeshipId = 1,
                        CourseName = FirstCourseName,
                        Amount = 123.4567M,
                        TransferDate = DateTime.Now
                    },
                    new AccountTransfer
                    {
                        SenderAccountId = SenderAccountId,
                        SenderAccountName = SenderAccountName,
                        ReceiverAccountId = ReceiverAccountId,
                        ReceiverAccountName = ReceiverAccountName,
                        ApprenticeshipId = 2,
                        CourseName = SecondCourseName,
                        Amount = 346.789M,
                        TransferDate = DateTime.Now
                    },
                    new AccountTransfer
                    {
                        SenderAccountId = SenderAccountId,
                        SenderAccountName = SenderAccountName,
                        ReceiverAccountId = ReceiverAccountId,
                        ReceiverAccountName = ReceiverAccountName,
                        ApprenticeshipId = 3,
                        CourseName = SecondCourseName,
                        Amount = 234.56M,
                        TransferDate = DateTime.Now
                    }
                };

            _db.Setup(d => d.SqlQueryAsync<AccountTransfer>(
                It.IsAny<string>(), SenderAccountId, ReceiverAccountId, PeriodEnd))
                .ReturnsAsync(_transfers);

            _db.Setup(d => d.SqlQueryAsync<AccountTransfer>(
                    It.IsAny<string>(), ReceiverAccountId, SenderAccountId, PeriodEnd))
                .ReturnsAsync(_transfers);

            _publicHashingService.Setup(x => x.HashValue(SenderAccountId))
                .Returns(SenderPublicHashedId);

            _publicHashingService.Setup(x => x.HashValue(ReceiverAccountId))
                .Returns(ReceiverPublicHashedId);
        }

        [TestCase(SenderAccountId, ReceiverAccountId)]
        [TestCase(ReceiverAccountId, SenderAccountId)]
        public async Task ThenIShouldReturnCorrectSenderDetails(long accountId, long targetAccountId)
        {
            //Assign
            _query = _query = new GetTransferTransactionDetailsQuery
            {
                AccountId = accountId,
                TargetAccountId = targetAccountId,
                PeriodEnd = PeriodEnd,
                UserId = UserId
            };

            //Act
            var result = await _handler.Handle(_query);

            //Assert
            Assert.AreEqual(SenderAccountName, result.SenderAccountName);
            Assert.AreEqual(SenderPublicHashedId, result.SenderPublicHashedId);
        }

        [TestCase(SenderAccountId, ReceiverAccountId)]
        [TestCase(ReceiverAccountId, SenderAccountId)]
        public async Task ThenIShouldReturnCorrectReceiverDetails(long accountId, long targetAccountId)
        {
            //Assign
            _query = _query = new GetTransferTransactionDetailsQuery
            {
                AccountId = accountId,
                TargetAccountId = targetAccountId,
                PeriodEnd = PeriodEnd,
                UserId = UserId
            };

            //Act
            var result = await _handler.Handle(_query);

            //Assert
            Assert.AreEqual(ReceiverAccountName, result.ReceiverAccountName);
            Assert.AreEqual(ReceiverPublicHashedId, result.ReceiverPublicHashedId);
        }

        [TestCase(SenderAccountId, ReceiverAccountId)]
        [TestCase(ReceiverAccountId, SenderAccountId)]
        public async Task ThenIShouldReturnCorrectCourseDetails(long accountId, long targetAccountId)
        {
            //Assign
            _query = _query = new GetTransferTransactionDetailsQuery
            {
                AccountId = accountId,
                TargetAccountId = targetAccountId,
                PeriodEnd = PeriodEnd,
                UserId = UserId
            };

            //Act
            var result = await _handler.Handle(_query);

            //Assert
            Assert.AreEqual(2, result.TransferDetails.Count());
            Assert.IsTrue(result.TransferDetails.Any(t => t.CourseName.Equals(FirstCourseName)));
            Assert.IsTrue(result.TransferDetails.Any(t => t.CourseName.Equals(SecondCourseName)));
        }

        [TestCase(SenderAccountId, ReceiverAccountId)]
        [TestCase(ReceiverAccountId, SenderAccountId)]
        public async Task ThenIShouldReturnCorrectCourseSubTotals(long accountId, long targetAccountId)
        {
            //Assign
            _query = _query = new GetTransferTransactionDetailsQuery
            {
                AccountId = accountId,
                TargetAccountId = targetAccountId,
                PeriodEnd = PeriodEnd,
                UserId = UserId
            };

            //Act
            var result = await _handler.Handle(_query);

            //Assert
            Assert.AreEqual(2, result.TransferDetails.Count());

            var firstCourseTotal = result.TransferDetails.Single(t => t.CourseName.Equals(FirstCourseName)).PaymentTotal;
            var secondCourseTotal = result.TransferDetails.Single(t => t.CourseName.Equals(SecondCourseName)).PaymentTotal;

            var expectedFirstCourseTotal =
                _transfers.Where(t => t.CourseName.Equals(FirstCourseName)).Sum(x => x.Amount);

            var expectedSecondCourseTotal =
                _transfers.Where(t => t.CourseName.Equals(SecondCourseName)).Sum(x => x.Amount);

            Assert.AreEqual(expectedFirstCourseTotal, firstCourseTotal);
            Assert.AreEqual(expectedSecondCourseTotal, secondCourseTotal);
        }

        [TestCase(SenderAccountId, ReceiverAccountId)]
        [TestCase(ReceiverAccountId, SenderAccountId)]
        public async Task ThenIShouldReturnCorrectCourseApprenticeCount(long accountId, long targetAccountId)
        {
            //Assign
            _query = _query = new GetTransferTransactionDetailsQuery
            {
                AccountId = accountId,
                TargetAccountId = targetAccountId,
                PeriodEnd = PeriodEnd,
                UserId = UserId
            };

            //Act
            var result = await _handler.Handle(_query);

            //Assert

            var firstCourseApprenticeCount = result.TransferDetails.Single(t => t.CourseName.Equals(FirstCourseName))
                .ApprenticeCount;

            var secondCourseApprenticeCount = result.TransferDetails.Single(t => t.CourseName.Equals(SecondCourseName))
                .ApprenticeCount;

            var expectedFirstCourseApprenticeCount =
                _transfers.Count(t => t.CourseName.Equals(FirstCourseName));

            var expectedSecondCourseApprenticeCount =
                _transfers.Count(t => t.CourseName.Equals(SecondCourseName));

            Assert.AreEqual(expectedFirstCourseApprenticeCount, firstCourseApprenticeCount);
            Assert.AreEqual(expectedSecondCourseApprenticeCount, secondCourseApprenticeCount);
        }

        [TestCase(SenderAccountId, ReceiverAccountId)]
        [TestCase(ReceiverAccountId, SenderAccountId)]
        public async Task ThenIShouldReturnTransferPaymentTotal(long accountId, long targetAccountId)
        {
            //Assign
            _query = _query = new GetTransferTransactionDetailsQuery
            {
                AccountId = accountId,
                TargetAccountId = targetAccountId,
                PeriodEnd = PeriodEnd,
                UserId = UserId
            };

            //Act
            var result = await _handler.Handle(_query);

            //Assert
            var expectedPaymentTotal = _transfers.Sum(t => t.Amount);

            Assert.AreEqual(expectedPaymentTotal, result.TransferPaymentTotal);
        }

        [TestCase(SenderAccountId, ReceiverAccountId)]
        [TestCase(ReceiverAccountId, SenderAccountId)]
        public async Task ThenIShouldReturnTransferDate(long accountId, long targetAccountId)
        {
            //Assign
            _query = _query = new GetTransferTransactionDetailsQuery
            {
                AccountId = accountId,
                TargetAccountId = targetAccountId,
                PeriodEnd = PeriodEnd,
                UserId = UserId
            };

            //Act
            var result = await _handler.Handle(_query);

            //Assert
            var expectedTransferDate = _transfers.First().TransferDate;

            Assert.AreEqual(expectedTransferDate, result.DateCreated);
        }
    }
}

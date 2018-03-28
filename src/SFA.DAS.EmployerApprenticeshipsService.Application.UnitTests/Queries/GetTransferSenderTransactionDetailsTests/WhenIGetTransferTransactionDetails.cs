using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Hashing;
using SFA.DAS.EAS.Application.Queries.GetTransferSenderTransactionDetails;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransferSenderTransactionDetailsTests
{
    class WhenIGetTransferTransactionDetails
    {
        private const string ReceiverAccountName = "Test Receiver";
        private const string FirstCourseName = "Course 1";
        private const string SecondCourseName = "Course 2";

        private GetTransferSenderTransactionDetailsQueryHandler _handler;
        private GetTransferSenderTransactionDetailsQuery _query;
        private GetTransferSenderTransactionDetailsResponse _response;
        private Mock<EmployerFinancialDbContext> _db;
        private List<AccountTransfer> _transfers;
        private Mock<IPublicHashingService> _publicHashingService;

        [SetUp]
        public void Assign()
        {
            _db = new Mock<EmployerFinancialDbContext>();

            _publicHashingService = new Mock<IPublicHashingService>();

            _query = new GetTransferSenderTransactionDetailsQuery
            {
                AccountId = 5,
                ReceiverAccountId = 12,
                PeriodEnd = "1718-R01",
                UserId = 45
            };
            _response = new GetTransferSenderTransactionDetailsResponse();

            _handler = new GetTransferSenderTransactionDetailsQueryHandler(_db.Object, _publicHashingService.Object);

            _transfers = new List<AccountTransfer>
                {
                    new AccountTransfer
                    {
                        SenderAccountId = _query.AccountId.GetValueOrDefault(),
                        ReceiverAccountId = _query.ReceiverAccountId,
                        ReceiverAccountName = ReceiverAccountName,
                        ApprenticeshipId = 1,
                        CourseName = FirstCourseName,
                        Amount = 123.4567M
                    },
                    new AccountTransfer
                    {
                        SenderAccountId = _query.AccountId.GetValueOrDefault(),
                        ReceiverAccountId = _query.ReceiverAccountId,
                        ReceiverAccountName = ReceiverAccountName,
                        ApprenticeshipId = 2,
                        CourseName = SecondCourseName,
                        Amount = 346.789M
                    },

                    new AccountTransfer
                    {
                        SenderAccountId = _query.AccountId.GetValueOrDefault(),
                        ApprenticeshipId = 3,
                        ReceiverAccountId = _query.ReceiverAccountId,
                        ReceiverAccountName = ReceiverAccountName,
                        CourseName = SecondCourseName,
                        Amount = 234.56M
                    },

                };

            _db.Setup(d => d.SqlQueryAsync<AccountTransfer>(
                It.IsAny<string>(), It.IsAny<long>(), It.IsAny<long>(), It.IsAny<string>()))
                .ReturnsAsync(_transfers);
        }

        [Test]
        public async Task ThenIShouldGetTransferDetails()
        {
            //Act
            await _handler.Handle(_query);


            //Assert
            _db.Verify(d => d.SqlQueryAsync<AccountTransfer>(
                It.Is<string>(q => q.Contains("[employer_financial].[GetTransferSenderTransactionDetails]")),
                _query.AccountId.Value,
                _query.ReceiverAccountId,
                _query.PeriodEnd), Times.Once);
        }

        [Test]
        public async Task ThenIShouldReturnCorrectReceiverDetails()
        {
            //Assign
            var expectedReceiverPublicHashedId = "GFG3242";

            _publicHashingService.Setup(x => x.HashValue(_query.ReceiverAccountId))
                                 .Returns(expectedReceiverPublicHashedId);

            //Act
            var result = await _handler.Handle(_query);


            //Assert
            Assert.AreEqual(ReceiverAccountName, result.ReceiverAccountName);
            Assert.AreEqual(expectedReceiverPublicHashedId, result.ReceiverPublicHashedId);
        }

        [Test]
        public async Task ThenIShouldReturnCorrectCourseDetails()
        {
            //Act
            var result = await _handler.Handle(_query);


            //Assert
            Assert.AreEqual(2, result.TransferDetails.Count());
            Assert.IsTrue(result.TransferDetails.Any(t => t.CourseName.Equals(FirstCourseName)));
            Assert.IsTrue(result.TransferDetails.Any(t => t.CourseName.Equals(SecondCourseName)));
        }

        [Test]
        public async Task ThenIShouldReturnCorrectCourseSubTotals()
        {
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

        [Test]
        public async Task ThenIShouldReturnCorrectCourseApprenticeCount()
        {
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
    }
}

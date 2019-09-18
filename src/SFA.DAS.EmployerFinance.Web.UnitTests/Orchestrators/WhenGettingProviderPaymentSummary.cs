using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Account.Api.Client;
using SFA.DAS.EmployerFinance.Interfaces;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.Payments;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.FindAccountProviderPayments;
using SFA.DAS.EmployerFinance.Queries.GetEmployerAccount;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Orchestrators
{
    [Parallelizable]
    public class WhenGettingProviderPaymentSummary
    {
        private IFixture _fixture = new Fixture();
        private EmployerAccountTransactionsOrchestrator _sut;
        private Mock<IAccountApiClient> _accountApiMock;
        private Mock<IMediator> _mediatorMock;
        private Mock<ICurrentDateTime> _currentTimeMock;

        [SetUp]
        public void SetUp()
        {
            _mediatorMock = new Mock<IMediator>();
            _accountApiMock = new Mock<IAccountApiClient>();
            _currentTimeMock = new Mock<ICurrentDateTime>();

            _mediatorMock
                .Setup(mock => mock.SendAsync(It.IsAny<GetEmployerAccountHashedQuery>()))
                .ReturnsAsync(new GetEmployerAccountResponse
                {
                    Account = new Account { ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy }
                });

            SetupGetCoursePaymentsResponse(2019, 9);

            _sut = new EmployerAccountTransactionsOrchestrator(
                _accountApiMock.Object,
                _mediatorMock.Object,
                _currentTimeMock.Object,
                Mock.Of<ILog>());
        }

        [Test]
        [TestCase(1, Description = "Provider payment summary for single course")]
        [TestCase(9, Description = "Provider payment summary for multiple courses")]
        public async Task ThenSummariesForEachCourseShouldBeCreated(int numberOfCourses)
        {
            // Arrange
            var coursePayments = CreateCoursePayments(numberOfCourses, 1);
            SetupGetProviderPaymentsResponse(2019, 9, coursePayments);

            // Act
            var response = await _sut.GetProviderPaymentSummary("abc123", 888888, new DateTime(2019, 9, 1), new DateTime(2019, 9, 30), "userId");

            // Assert
            response.Data.CoursePayments.Count().Should().Be(numberOfCourses);
        }

        [Test]
        public async Task ThenNonLevyEmployerShouldNotSeeNonCoInvestmentPaymentColumn_IfThereIsNoValue()
        {
            // Arrange
            _mediatorMock
                .Setup(mock => mock.SendAsync(It.IsAny<GetEmployerAccountHashedQuery>()))
                .ReturnsAsync(new GetEmployerAccountResponse
                {
                    Account = new Account { ApprenticeshipEmployerType = ApprenticeshipEmployerType.NonLevy }
                });

            var coursePayments = CreateCoursePayments(1, 1, 0, 900, 100);

            foreach (var coursePayment in coursePayments)
            {
                coursePayment.LineAmount = 0;
                coursePayment.EmployerCoInvestmentAmount = 100;
                coursePayment.SfaCoInvestmentAmount = 900;
            }

            SetupGetProviderPaymentsResponse(2019, 9, coursePayments);

            // Act
            var response = await _sut.GetCoursePaymentSummary("abc123", 888888, "A course", 4, null, new DateTime(2019, 9, 1), new DateTime(2019, 9, 30), "userId");

            // Assert
            response.Data.ShowNonCoInvesmentPaymentsTotal.Should().BeFalse();
        }

        [Test]
        public async Task ThenLevyEmployerShouldSeeNonCoInvestmentPaymentColumn_IfThereIsNoValue()
        {
            // Arrange
            _mediatorMock
                .Setup(mock => mock.SendAsync(It.IsAny<GetEmployerAccountHashedQuery>()))
                .ReturnsAsync(new GetEmployerAccountResponse
                {
                    Account = new Account { ApprenticeshipEmployerType = ApprenticeshipEmployerType.Levy }
                });

            var coursePayments = CreateCoursePayments(1, 1, 0, 900, 100);

            foreach (var coursePayment in coursePayments)
            {
                coursePayment.LineAmount = 0;
                coursePayment.EmployerCoInvestmentAmount = 100;
                coursePayment.SfaCoInvestmentAmount = 900;
            }

            SetupGetProviderPaymentsResponse(2019, 9, coursePayments);

            // Act
            var response = await _sut.GetCoursePaymentSummary("abc123", 888888, "A course", 4, null, new DateTime(2019, 9, 1), new DateTime(2019, 9, 30), "userId");

            // Assert
            response.Data.ShowNonCoInvesmentPaymentsTotal.Should().BeTrue();
        }

        [Test]
        [TestCase(ApprenticeshipEmployerType.Levy)]
        [TestCase(ApprenticeshipEmployerType.NonLevy)]
        public async Task ThenUserShouldSeeNonCoInvestmentPaymentColumn_IfThereIsValue(ApprenticeshipEmployerType apprenticeshipEmployerType)
        {
            // Arrange
            _mediatorMock
                .Setup(mock => mock.SendAsync(It.IsAny<GetEmployerAccountHashedQuery>()))
                .ReturnsAsync(new GetEmployerAccountResponse
                {
                    Account = new Account { ApprenticeshipEmployerType = apprenticeshipEmployerType }
                });

            var coursePayments = CreateCoursePayments(1, 1, 1000, 0, 0);

            SetupGetProviderPaymentsResponse(2019, 9, coursePayments);

            // Act
            var response = await _sut.GetCoursePaymentSummary("abc123", 888888, "A course", 4, null, new DateTime(2019, 9, 1), new DateTime(2019, 9, 30), "userId");

            // Assert
            response.Data.ShowNonCoInvesmentPaymentsTotal.Should().BeTrue();
        }

        private IEnumerable<PaymentTransactionLine> CreateCoursePayments(
            int noOfCourses,
            int noOfPaymentsForCourse,
            decimal lineAmount = 100,
            decimal sfaCoInvestment = 100,
            decimal employerCoInvestment = 100)
        {
            var payments = new List<PaymentTransactionLine>();

            for (int i = 1; i <= noOfCourses; i++)
            {
                payments.AddRange(_fixture
                    .Build<PaymentTransactionLine>()
                    .Without(ptl => ptl.SubTransactions)
                    .With(ptl => ptl.TransactionType, TransactionItemType.Payment)
                    .With(ptl => ptl.ApprenticeName, $"Apprentice-{0}")
                    .With(ptl => ptl.ApprenticeNINumber, $"ApprenticeNI-{0}")
                    .With(ptl => ptl.ApprenticeULN, i)
                    .With(ptl => ptl.LineAmount, lineAmount)
                    .With(ptl => ptl.SfaCoInvestmentAmount, sfaCoInvestment)
                    .With(ptl => ptl.EmployerCoInvestmentAmount, employerCoInvestment)
                    .CreateMany(noOfPaymentsForCourse));
            }

            return payments;
        }

        private void SetupGetCoursePaymentsResponse(int year, int month)
        {
            SetupGetProviderPaymentsResponse(year, month, new PaymentTransactionLine[0]);
        }

        private void SetupGetProviderPaymentsResponse(int year, int month, IEnumerable<PaymentTransactionLine> payments)
        {
            _mediatorMock.Setup(x => x.SendAsync(It.IsAny<FindAccountProviderPaymentsQuery>()))
                .ReturnsAsync(new FindAccountProviderPaymentsResponse
                {
                    Transactions = payments.ToList()
                });
        }
    }
}

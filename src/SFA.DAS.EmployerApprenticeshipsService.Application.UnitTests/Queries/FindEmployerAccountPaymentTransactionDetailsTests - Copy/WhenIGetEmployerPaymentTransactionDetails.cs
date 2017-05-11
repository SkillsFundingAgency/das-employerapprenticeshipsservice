using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.FindAccountProviderPayments;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.ApprenticeshipProvider;
using SFA.DAS.EAS.Domain.Models.Payments;

namespace SFA.DAS.EAS.Application.UnitTests.Queries
{
    public class WhenIGetEmployerPaymentTransactionDetails : QueryBaseTest<FindAccountProviderPaymentsHandler, FindAccountProviderPaymentsQuery, FindAccountProviderPaymentsResponse>
    {
        private const string ProviderName = "Test Provider";

        private Mock<IDasLevyService> _dasLevyService;
        private Mock<IHashingService> _hashingService;
        private DateTime _fromDate;
        private DateTime _toDate;
        private long _accountId;
        private string _hashedAccountId;
        private string _externalUserId;
      
        private Mock<IApprenticeshipInfoServiceWrapper> _apprenticeshipInfoService;
        public override FindAccountProviderPaymentsQuery Query { get; set; }
        public override FindAccountProviderPaymentsHandler RequestHandler { get; set; }
        public override Mock<IValidator<FindAccountProviderPaymentsQuery>> RequestValidator { get; set; }
       
        [SetUp]
        public void Arrange()
        {
            SetUp();

            _fromDate = DateTime.Now.AddDays(-10);
            _toDate = DateTime.Now.AddDays(-2);
            _accountId = 1;
            _hashedAccountId = "123ABC";
            _externalUserId = "test";

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>())).Returns(_accountId);

            _dasLevyService = new Mock<IDasLevyService>();
            _dasLevyService.Setup(x => x.GetAccountProviderPaymentsByDateRange<PaymentTransactionLine>
                                            (It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                           .ReturnsAsync(new List<PaymentTransactionLine>
                {
                    new PaymentTransactionLine { ProviderName = ProviderName }
                });

            Query = new FindAccountProviderPaymentsQuery
            {
                HashedAccountId = _hashedAccountId,
                FromDate = _fromDate,
                ToDate = _toDate,
                ExternalUserId = _externalUserId
            };

            _apprenticeshipInfoService = new Mock<IApprenticeshipInfoServiceWrapper>();
            _apprenticeshipInfoService.Setup(x => x.GetProvider(It.IsAny<int>()))
                .Returns(new ProvidersView
                {
                    Provider = new Domain.Models.ApprenticeshipProvider.Provider()
                        {
                            Name = ProviderName
                        }
                });

            RequestHandler = new FindAccountProviderPaymentsHandler(
                RequestValidator.Object, 
                _dasLevyService.Object,
                _hashingService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _hashingService.Verify(x => x.DecodeValue(_hashedAccountId), Times.Once);
            _dasLevyService.Verify(x=>x.GetAccountProviderPaymentsByDateRange<PaymentTransactionLine>
                                            (_accountId, _fromDate, _toDate, _externalUserId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual.Transactions);
        }

        [Test]
        public void ThenAnUnauhtorizedExceptionIsThrownIfTheValidationResultReturnsUnauthorized()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<FindAccountProviderPaymentsQuery>()))
                            .ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new FindAccountProviderPaymentsQuery()));
        }

        [Test]
        public async Task ThenTheProviderNameShouldBeAddedToTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(ProviderName, actual.ProviderName);
        }

        [Test]
        public async Task ThenTheTransactionDateShouldBeAddedToTheResponse()
        {
            //Arrange
            var transactionDate = DateTime.Now.AddDays(-2);
            _dasLevyService.Setup(x => x.GetAccountProviderPaymentsByDateRange<PaymentTransactionLine>
                                           (It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                           .ReturnsAsync(new List<PaymentTransactionLine>
               {
                    new PaymentTransactionLine {TransactionDate = transactionDate}
               });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(transactionDate, actual.TransactionDate);
        }

        [Test]
        public void ThenANotFoundExceptionShouldBeThrowIfNoTransactionsAreFound()
        {
            //Arrange
            _dasLevyService.Setup(x => x.GetAccountProviderPaymentsByDateRange<PaymentTransactionLine>
                    (It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                .ReturnsAsync(new List<PaymentTransactionLine>());

            //Act
            Assert.ThrowsAsync<NotFoundException>(async () => await RequestHandler.Handle(Query));
        }
    }
}

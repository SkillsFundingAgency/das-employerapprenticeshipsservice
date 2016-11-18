using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAccountTransactionDetail;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Levy;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAccountTransactionDetailsTests
{
    public class WhenIGetEmployerTransactionDetails : QueryBaseTest<GetEmployerAccountTransactionDetailHandler, GetAccountLevyDeclarationTransactionsByDateRangeQuery, GetAccountLevyDeclarationTransactionsByDateRangeResponse>
    {
        private Mock<IDasLevyService> _dasLevyService;
        private Mock<IHashingService> _hashingService;
        private DateTime _fromDate;
        private DateTime _toDate;
        private long _accountId;
        private string _hashedAccountId;
        private string _externalUserId;
        public override GetAccountLevyDeclarationTransactionsByDateRangeQuery Query { get; set; }
        public override GetEmployerAccountTransactionDetailHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountLevyDeclarationTransactionsByDateRangeQuery>> RequestValidator { get; set; }
       
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
            _dasLevyService.Setup(x => x.GetTransactionsByDateRange<LevyDeclarationTransactionLine>
                                            (It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                           .ReturnsAsync(new List<LevyDeclarationTransactionLine>()
                {
                    new LevyDeclarationTransactionLine()
                });

            Query = new GetAccountLevyDeclarationTransactionsByDateRangeQuery
            {
                HashedAccountId = _hashedAccountId,
                FromDate = _fromDate,
                ToDate = _toDate,
                ExternalUserId = _externalUserId
            };

            RequestHandler = new GetEmployerAccountTransactionDetailHandler(
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
            _dasLevyService.Verify(x=>x.GetTransactionsByDateRange<LevyDeclarationTransactionLine>
                                            (_accountId, _fromDate, _toDate, _externalUserId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.IsNotEmpty(actual.TransactionDetail);
        }

        [Test]
        public void ThenAnUnauhtorizedExceptionIsThrownIfTheValidationResultReturnsUnauthorized()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountLevyDeclarationTransactionsByDateRangeQuery>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetAccountLevyDeclarationTransactionsByDateRangeQuery()));
        }

        [Test]
        public async Task ThenTheLineItemTotalIsCalculatedFromTheAmountTopupAndPercentageOfFraction()
        {
            //Arrange
            _dasLevyService.Setup(x => x.GetTransactionsByDateRange<LevyDeclarationTransactionLine>
                                            (It.IsAny<long>(), It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<string>()))
                           .ReturnsAsync(new List<LevyDeclarationTransactionLine>
            {
                new LevyDeclarationTransactionLine {Amount=10,EnglishFraction = 0.5m,TransactionType = TransactionItemType.Declaration},
                new LevyDeclarationTransactionLine {Amount=1,EnglishFraction = 0.5m,TransactionType = TransactionItemType.TopUp}
            });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(11,actual.Total);
        }
    }
}

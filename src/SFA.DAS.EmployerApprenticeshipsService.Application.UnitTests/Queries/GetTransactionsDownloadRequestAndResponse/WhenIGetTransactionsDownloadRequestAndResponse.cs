using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Models.Transaction;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetTransactionsDownloadRequestAndResponse
{
    public class WhenIGetTransactionsDownloadRequestAndResponse : QueryBaseTest<GetTransactionsDownloadRequestAndResponseHandler, Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse, Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse>
    {
        private Mock<ITransactionRepository> _transactionsRepository;
        private Mock<ITransactionFormatterFactory> _transactionFormatterFactory;
        private Mock<IMembershipRepository> _membershipRepository;

        public override Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse Query { get; set; }

        public override GetTransactionsDownloadRequestAndResponseHandler RequestHandler { get; set; }

        public override Mock<IValidator<Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse>> RequestValidator { get; set; }

        private const long ExpectedAccountId = 4564878;
        private const string ExpectedUserId = "123dfv";

        private readonly TransactionsDownloadStartDateMonthYearDateTime _fromDate = new TransactionsDownloadStartDateMonthYearDateTime
        {
            Month = 1,
            Year = 2000,
            //(2000, 1, 1)
        };

        private readonly TransactionsDownloadEndDateMonthYearDateTime _toDate = new TransactionsDownloadEndDateMonthYearDateTime
        {
            Month = 1,
            Year = 2000,
            //(2000, 1, 1)
        };

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _membershipRepository = new Mock<IMembershipRepository>();

            _transactionsRepository = new Mock<ITransactionRepository>();

            _transactionFormatterFactory = new Mock<ITransactionFormatterFactory>();

            _transactionsRepository
                .Setup(x => x.GetAllTransactionDetailsForAccountByDate(ExpectedAccountId,
                    _fromDate.ToDateTime(), _toDate.ToDateTime()))
                .ReturnsAsync(new List<TransactionDownloadLine>
                {
                    new TransactionDownloadLine()
                });

            RequestHandler = new GetTransactionsDownloadRequestAndResponseHandler(_transactionsRepository.Object,
                RequestValidator.Object, _transactionFormatterFactory.Object, _membershipRepository.Object);

            Query = new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse();
        }

        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                AccountId = ExpectedAccountId,
                ExternalUserId = ExpectedUserId,
                StartDate = _fromDate,
                EndDate = _toDate
            });

            //Assert
            _transactionsRepository.Verify(x =>
                x.GetAllTransactionDetailsForAccountByDate(ExpectedAccountId, _fromDate.ToDateTime(),
                    _toDate.ToDateTime()));
        }

        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var result = await RequestHandler.Handle(new Application.Queries.GetTransactionsDownloadResultViewModel.GetTransactionsDownloadRequestAndResponse
            {
                AccountId = ExpectedAccountId,
                ExternalUserId = ExpectedUserId,
                StartDate = _fromDate,
                EndDate = _toDate
            });

            //Assert
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.Transactions);
        }
    }
}

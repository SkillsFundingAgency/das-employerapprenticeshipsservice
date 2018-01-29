using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.HashingService;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountTransactionsController
{
    public class WhenIDownloadTransactionsByDate
    {
        private Web.Controllers.EmployerAccountTransactionsController _controller;
        private Mock<EmployerAccountTransactionsOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggleService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private Mock<ITransactionFormatter> _formatter;
        private Mock<IHashingService> _hashingService;
        private Mock<IMediator> _mediator
            ;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggleService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _formatter = new Mock<ITransactionFormatter>();
            _formatter.Setup(x => x.GetFileData(It.IsAny<List<TransactionDownloadLine>>()))
                .Returns(new byte[] {1, 2, 3, 4});
            _formatter.Setup(x => x.MimeType).Returns("txt/csv");
            _formatter.Setup(x => x.DownloadFormatType).Returns(DownloadFormatType.Csv);

            _hashingService = new Mock<IHashingService>();
            _mediator = new Mock<IMediator>();

            var transactionFormatterFactory = new Mock<ITransactionFormatterFactory>();

            transactionFormatterFactory.Setup(x => x.GetTransactionsFormatterByType(It.IsAny<DownloadFormatType>()))
                .Returns(_formatter.Object);

            _controller = new Web.Controllers.EmployerAccountTransactionsController(_owinWrapper.Object,
                _featureToggle.Object,
                _hashingService.Object,
                _mediator.Object,
                _orchestrator.Object, _userViewTestingService.Object, _flashMessage.Object,
                transactionFormatterFactory.Object);
        }

        [Test]
        public async Task WithAnInvalidFromMonthThenValidStartDateIsFalseAndTransactionsAreNotRetrievedForTheAccount()
        {
            var result = await _controller.TransactionDownloadByDate("HashedAccountId",
                new GetTransactionsDownloadRequestAndResponse
                {
                    StartDate = new TransactionsDownloadStartDateMonthYearDateTime()
                    {
                        Month = -1,
                        Year = 2000
                    },
                    EndDate = new TransactionsDownloadEndDateMonthYearDateTime
                    {
                        Month = -1,
                        Year = 2000
                    },
                }
                ) as ViewResultBase;

            Assert.IsNotNull(result);

            var viewModel = result.Model as GetTransactionsDownloadRequestAndResponse;
            Assert.IsNotNull(viewModel);
            Assert.IsNotNull(viewModel);
            Assert.IsFalse(viewModel.StartDate.Valid);
            Assert.IsNull(viewModel.Transactions);

            _mediator.Verify(
                x => x.SendAsync(It.IsAny<GetTransactionsDownloadRequestAndResponse>()), Times.Never);
        }

        [Test]
        public async Task WithAnInvalidFromYearThenValidStartDateIsFalseAndTransactionsAreNotRetrievedForTheAccount()
        {
            var result = await _controller.TransactionDownloadByDate("HashedAccountId",
                new GetTransactionsDownloadRequestAndResponse
                {
                    StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                    {
                        Month = 1,
                        Year = -1
                    },
                    EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                    {
                        Month = 1,
                        Year = 2000
                    },
                }) as ViewResultBase;

            Assert.IsNotNull(result);

            var viewModel = result.Model as GetTransactionsDownloadRequestAndResponse;
            Assert.IsNotNull(viewModel);
            Assert.IsFalse(viewModel.StartDate.Valid);
            Assert.IsNull(viewModel.Transactions);

            _mediator.Verify(
                x => x.SendAsync(It.IsAny<GetTransactionsDownloadRequestAndResponse>()), Times.Never);
        }

        [Test]
        public async Task WithAnInvalidEndMonthThenValidToDateIsFalseAndTransactionsAreNotRetrievedForTheAccount()
        {
            var result = await _controller.TransactionDownloadByDate("HashedAccountId",
                new GetTransactionsDownloadRequestAndResponse
                {
                    StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                    {
                        Month = 1,
                        Year = 2000
                    },
                    EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                    {
                        Month = -1,
                        Year = 2000
                    },
                }) as ViewResultBase;

            Assert.IsNotNull(result);

            var viewModel = result.Model as GetTransactionsDownloadRequestAndResponse;
            Assert.IsNotNull(viewModel);
            Assert.IsFalse(viewModel.EndDate.Valid);
            Assert.IsNull(viewModel.Transactions);

            _mediator.Verify(
                x => x.SendAsync(It.IsAny<GetTransactionsDownloadRequestAndResponse>()), Times.Never);
        }

        [Test]
        public async Task WithAnInvalidtoYearThenValidToDateIsFalseAndTransactionsAreNotRetrievedForTheAccount()
        {
            var result = await _controller.TransactionDownloadByDate("HashedAccountId",
                new GetTransactionsDownloadRequestAndResponse
                {
                    StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                    {
                        Month = 1,
                        Year = 2000
                    },
                    EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                    {
                        Month = 1,
                        Year = -1
                    },
                }) as ViewResultBase;

            Assert.IsNotNull(result);

            var viewModel = result.Model as GetTransactionsDownloadRequestAndResponse;
            Assert.IsNotNull(viewModel);
            Assert.IsFalse(viewModel.EndDate.Valid);
            Assert.IsNull(viewModel.Transactions);

            _mediator.Verify(
                x => x.SendAsync(It.IsAny<GetTransactionsDownloadRequestAndResponse>()), Times.Never);
        }

        [Test]
        public async Task WithValidDatesThenTransactionsAreRetrievedForTheAccount()
        {
            const int fromMonth = 1;
            const int fromYear = 2000;
            const int toMonth = 1;
            const int toYear = 2018;

            _mediator.Setup(
                    x => x.SendAsync(It.IsAny<GetTransactionsDownloadRequestAndResponse>()))
                .ReturnsAsync(new GetTransactionsDownloadRequestAndResponse
                {
                    StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                    {
                        Month = fromMonth,
                        Year = fromYear,
                    },
                    EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                    {
                        Month = toMonth,
                        Year = toYear,
                    },
                    Transactions = new List<TransactionDownloadLine>()
                    {
                        new TransactionDownloadLine()
                    },
                    MimeType = @"text/csv",
                    FileExtension = "hello",
                    FileDate = new byte[]{},
                });

            var result = await _controller.TransactionDownloadByDate("HashedAccountId",
                new GetTransactionsDownloadRequestAndResponse
                {
                    StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                    {
                        Month = fromMonth,
                        Year = fromYear
                    },
                    EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                    {
                        Month = toMonth,
                        Year = toYear
                    },
                }) as FileContentResult;

            Assert.IsNotNull(result);

            _mediator.Verify(
                x => x.SendAsync(It.IsAny<GetTransactionsDownloadRequestAndResponse>()), Times.Once);
        }


        [Test]
        public async Task WithValidDatesThatFindNoResultsThenViewModelIsReturned()
        {
            const int fromMonth = 1;
            const int fromYear = 2000;
            const int toMonth = 1;
            const int toYear = 2018;

            _mediator.Setup(
                    x => x.SendAsync(It.IsAny<GetTransactionsDownloadRequestAndResponse>()))
                .ReturnsAsync(new GetTransactionsDownloadRequestAndResponse
                {
                    StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                    {
                        Month = fromMonth,
                        Year = fromYear,
                    },
                    EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                    {
                        Month = toMonth,
                        Year = toYear,
                    },
                    Transactions = new List<TransactionDownloadLine>()
                });

            var result = await _controller.TransactionDownloadByDate("HashedAccountId",
                new GetTransactionsDownloadRequestAndResponse
                {
                    StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                    {
                        Month = fromMonth,
                        Year = fromYear
                    },
                    EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                    {
                        Month = toMonth,
                        Year = toYear
                    },
                });

            Assert.IsNull(result as FileContentResult);
            Assert.IsNotNull(result as ViewResultBase);

            _mediator.Verify(
                x => x.SendAsync(It.IsAny<GetTransactionsDownloadRequestAndResponse>()), Times.Once);
        }

        [Test]
        public async Task WithValidDatesButInvalidAccountIdThenUnauthorizedIsReturned()
        {
            const string expectedAction = "Index";
            const string expectedController = "AccessDenied";

            const int fromMonth = 1;
            const int fromYear = 2000;
            const int toMonth = 1;
            const int toYear = 2018;

            _mediator.Setup(
                    x => x.SendAsync(It.IsAny<GetTransactionsDownloadRequestAndResponse>()))
                .ReturnsAsync(new GetTransactionsDownloadRequestAndResponse
                {
                    StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                    {
                        Month = fromMonth,
                        Year = fromYear,
                    },
                    EndDate = new TransactionsDownloadEndDateMonthYearDateTime()
                    {
                        Month = toMonth,
                        Year = toYear,
                    },
                    Transactions = new List<TransactionDownloadLine>()
                    {
                        new TransactionDownloadLine()
                    },
                    IsUnauthorized = true
                });

            var result = await _controller.TransactionDownloadByDate("HashedAccountId",
                new GetTransactionsDownloadRequestAndResponse
                {
                    StartDate = new TransactionsDownloadStartDateMonthYearDateTime
                    {
                        Month = fromMonth,
                        Year = fromYear
                    },
                    EndDate = new TransactionsDownloadEndDateMonthYearDateTime
                    {
                        Month = toMonth,
                        Year = toYear
                    },
                }) as RedirectToRouteResult;

            Assert.IsNotNull(result);

            Assert.AreEqual(expectedAction, result.RouteValues["action"]);
            Assert.AreEqual(expectedController, result.RouteValues["controller"]);
        }
    }
}

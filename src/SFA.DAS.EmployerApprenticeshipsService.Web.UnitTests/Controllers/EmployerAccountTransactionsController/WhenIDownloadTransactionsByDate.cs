using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Application.Validation;
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
        private readonly byte[] _expectedFileData = new byte[] { };

        private const string ExpectedFileExtension = "hello";

        private const string ExpectedMimeType = @"text/csv";


        private Web.Controllers.EmployerAccountTransactionsController _controller;
        private Mock<EmployerAccountTransactionsOrchestrator> _orchestrator;
        private Mock<IOwinWrapper> _owinWrapper;
        private Mock<IFeatureToggleService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private Mock<ITransactionFormatter> _formatter;
        private Mock<IHashingService> _hashingService;
        private Mock<IMediator> _mediator;
        private TransactionDownloadViewModel _transactionDownloadViewModel;

        [SetUp]
        public void Arrange()
        {
            _transactionDownloadViewModel = new TransactionDownloadViewModel
            {
                AccountHashedId = "",
                StartDate = new TransactionsDownloadStartDateMonthYearDateTime()
                {
                    Month = 1,
                    Year = 2000
                },
                EndDate = new TransactionsDownloadEndDateMonthYearDateTime
                {
                    Month = 1,
                    Year = 2018
                },
                Message = new GetTransactionsDownloadQuery()
            };
            _mediator = new Mock<IMediator>();

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetTransactionsDownloadQuery>()))
                .ReturnsAsync(new GetTransactionsDownloadResponse()
                {
                    ValidationResult = new ValidationResult
                    {
                        ValidationDictionary = new Dictionary<string, string>()
                    },
                    MimeType = ExpectedMimeType,
                    FileExtension = ExpectedFileExtension,
                    FileDate = _expectedFileData,
                });


            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
            _owinWrapper = new Mock<IOwinWrapper>();
            _featureToggle = new Mock<IFeatureToggleService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();

            _formatter = new Mock<ITransactionFormatter>();
            _formatter.Setup(x => x.GetFileData(It.IsAny<List<TransactionDownloadLine>>()))
                .Returns(new byte[] {1, 2, 3, 4});
            _formatter.Setup(x => x.MimeType).Returns("txt/csv");
            _formatter.Setup(x => x.DownloadFormatType).Returns(DownloadFormatType.CSV);

            _hashingService = new Mock<IHashingService>();

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
        public async Task ThenAGetTransactionsDownloadQueryShouldBeSent()
        {
            await _controller.TransactionDownloadByDate(_transactionDownloadViewModel);

            _mediator.Verify(m => m.SendAsync(_transactionDownloadViewModel.Message), Times.Once);
        }


        [Test]
        public async Task ThenTheModelStateShouldBeValid()
        {
            await _controller.TransactionDownloadByDate(_transactionDownloadViewModel);

            Assert.That(_controller.ModelState.IsValid, Is.True);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheSendTransferConnectionPage()
        {
            var result = await _controller.TransactionDownloadByDate(_transactionDownloadViewModel) as FileContentResult;

            Assert.That(result, Is.Not.Null);

            Assert.AreEqual(result.ContentType, ExpectedMimeType);
            Assert.AreEqual(result.FileContents, _expectedFileData);
            Assert.IsTrue(result.FileDownloadName.EndsWith(ExpectedFileExtension));
        }


        [Test]
        public async Task ThenTheModelStateShouldNotBeValidIfErrorsAreReturned()
        {
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetTransactionsDownloadQuery>()))
                .ReturnsAsync(new GetTransactionsDownloadResponse()
                {
                    ValidationResult = new ValidationResult
                    {
                        ValidationDictionary = new Dictionary<string, string>
                        {
                            ["Foo"] = "Bar"
                        }
                    },
                });

            var result = await _controller.TransactionDownloadByDate(_transactionDownloadViewModel) as FileContentResult;

            Assert.That(_controller.ModelState.IsValid, Is.False);
        }


        [Test]
        public async Task ThenIShouldReturnThePage()
        {
            _mediator.Setup(m => m.SendAsync(It.IsAny<GetTransactionsDownloadQuery>()))
                .ReturnsAsync(new GetTransactionsDownloadResponse()
                {
                    ValidationResult = new ValidationResult
                    {
                        ValidationDictionary = new Dictionary<string, string>
                        {
                            ["Foo"] = "Bar"
                        }
                    },
                });

            var result = await _controller.TransactionDownloadByDate(_transactionDownloadViewModel) as ViewResultBase;

            Assert.That(result, Is.Not.Null);
        }
        

        private static ValidationResult PopulatedValidationResult()
        {
            var validationResult = new ValidationResult();
            validationResult.AddError("Something", "There are no transactions in the date range");
            return validationResult;
        }
    }
}

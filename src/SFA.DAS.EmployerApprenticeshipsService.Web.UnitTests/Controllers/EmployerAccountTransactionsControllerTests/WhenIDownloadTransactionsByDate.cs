using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Formatters.TransactionDowloads;
using SFA.DAS.EAS.Application.Messages;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownload;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transaction;
using SFA.DAS.EAS.Infrastructure.Authentication;
using SFA.DAS.EAS.Infrastructure.Authorization;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Controllers;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Transactions;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountTransactionsControllerTests
{
    [TestFixture]
    public class WhenIDownloadTransactionsByDate
    {
        private const string ExpectedFileExtension = "hello";
        private const string ExpectedMimeType = @"text/csv";
        private static readonly byte[] ExpectedFileData = { };

        private EmployerAccountTransactionsController _controller;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IAuthorizationService> _featureToggle;
        private Mock<IHashingService> _hashingService;
        private Mock<IMediator> _mediator;
        private Mock<EmployerAccountTransactionsOrchestrator> _orchestrator;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private Mock<ITransactionFormatter> _formatter;
        private TransactionDownloadViewModel _transactionDownloadViewModel;

        [SetUp]
        public void Arrange()
        {
            _transactionDownloadViewModel = new TransactionDownloadViewModel
            {
                GetTransactionsDownloadQuery = new GetTransactionsDownloadQuery
                {
                    StartDate = new MonthYear
                    {
                        Month = "1",
                        Year = "2000"
                    },
                    EndDate = new MonthYear
                    {
                        Month = "1",
                        Year = "2018"
                    }
                }
            };

            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IAuthorizationService>();
            _hashingService = new Mock<IHashingService>();
            _mediator = new Mock<IMediator>();
            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _formatter = new Mock<ITransactionFormatter>();

            _mediator.Setup(m => m.SendAsync(It.IsAny<GetTransactionsDownloadQuery>()))
                .ReturnsAsync(new GetTransactionsDownloadResponse
                {
                    MimeType = ExpectedMimeType,
                    FileExtension = ExpectedFileExtension,
                    FileData = ExpectedFileData
                });

            _formatter.Setup(x => x.GetFileData(It.IsAny<List<TransactionDownloadLine>>())).Returns(new byte[] { 1, 2, 3, 4 });
            _formatter.Setup(x => x.MimeType).Returns("txt/csv");
            _formatter.Setup(x => x.DownloadFormatType).Returns(DownloadFormatType.CSV);

            var transactionFormatterFactory = new Mock<ITransactionFormatterFactory>();

            transactionFormatterFactory.Setup(x => x.GetTransactionsFormatterByType(It.IsAny<DownloadFormatType>())).Returns(_formatter.Object);

            _controller = new EmployerAccountTransactionsController(
                _owinWrapper.Object,
                _featureToggle.Object,
                _hashingService.Object,
                _mediator.Object,
                _orchestrator.Object,
                _userViewTestingService.Object,
                _flashMessage.Object,
                transactionFormatterFactory.Object,
                Mock.Of<IMapper>());
        }

        [Test]
        public async Task ThenAGetTransactionsDownloadQueryShouldBeSent()
        {
            await _controller.TransactionsDownload(_transactionDownloadViewModel);

            _mediator.Verify(m => m.SendAsync(_transactionDownloadViewModel.GetTransactionsDownloadQuery), Times.Once);
        }

        [Test]
        public async Task ThenTheModelStateShouldBeValid()
        {
            await _controller.TransactionsDownload(_transactionDownloadViewModel);

            Assert.That(_controller.ModelState.IsValid, Is.True);
        }

        [Test]
        public async Task ThenIShouldBeRedirectedToTheSendTransferConnectionInvitationPage()
        {
            var result = await _controller.TransactionsDownload(_transactionDownloadViewModel) as FileContentResult;

            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(result.ContentType, ExpectedMimeType);
            Assert.AreEqual(result.FileContents, ExpectedFileData);
            Assert.IsTrue(result.FileDownloadName.EndsWith(ExpectedFileExtension));
        }
    }
}
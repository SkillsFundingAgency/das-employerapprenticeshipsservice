using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Formatters.TransactionDowloads;
using SFA.DAS.EmployerFinance.Messages;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Queries.GetTransactionsDownload;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.EmployerAccountTransactionsControllerTests
{
    [TestFixture]
    public class WhenIDownloadTransactionsByDate
    {
        private const string ExpectedFileExtension = "hello";
        private const string ExpectedMimeType = @"text/csv";
        private static readonly byte[] ExpectedFileData = { };

        private EmployerAccountTransactionsController _controller;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMediator> _mediator;
        private Mock<EmployerAccountTransactionsOrchestrator> _orchestrator;
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
            _mediator = new Mock<IMediator>();
            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
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

            _controller = new EmployerAccountTransactionsController(
                _owinWrapper.Object,
                _orchestrator.Object,
                Mock.Of<IMapper>(),
                _mediator.Object,
                Mock.Of<ILog>());
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
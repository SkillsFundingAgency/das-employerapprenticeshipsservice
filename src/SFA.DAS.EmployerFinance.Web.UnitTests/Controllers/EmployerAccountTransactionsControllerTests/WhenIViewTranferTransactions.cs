using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Models.Transfers;
using SFA.DAS.EmployerFinance.Queries.GetTransferTransactionDetails;
using SFA.DAS.EmployerFinance.Web.Controllers;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers.EmployerAccountTransactionsControllerTests
{
    class WhenIViewTranferTransactions
    {
        private const long SenderAccountId = 1;
        private const string ReceiverPublicHashedAccountId = "DEF456";
        private const string PeriodEnd = "1718-R01";

        private EmployerAccountTransactionsController _controller;
        private Mock<EmployerAccountTransactionsOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IMapper> _mapper;
        private Mock<IMediator> _mediator;
        private GetTransferTransactionDetailsQuery _query;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _mapper = new Mock<IMapper>();
            _mediator = new Mock<IMediator>();

            _controller = new EmployerAccountTransactionsController(
                _owinWrapper.Object,
                _orchestrator.Object,
                _mapper.Object,
                _mediator.Object,
                Mock.Of<ILog>());

            _query = new GetTransferTransactionDetailsQuery
            {
                AccountId = SenderAccountId,
                TargetAccountPublicHashedId = ReceiverPublicHashedAccountId,
                PeriodEnd = PeriodEnd
            };
        }

        [Test]
        public async Task ThenIShouldGetTransferDetails()
        {
            //Assign
            var expectedViewModel = new TransferTransactionDetailsViewModel
            {
                ReceiverAccountName = "Test Group",
                ReceiverAccountPublicHashedId = "GFH657",
                IsCurrentAccountSender = true,
                TransferDetails = new List<AccountTransferDetails>()
            };

            var response = new GetTransferTransactionDetailsResponse
            {
                IsCurrentAccountSender = true,
                ReceiverAccountName = "Test Group",
                ReceiverAccountPublicHashedId = "GFH657",
                TransferDetails = new List<AccountTransferDetails>()
            };

            _mapper.Setup(x => x.Map<TransferTransactionDetailsViewModel>(response))
                   .Returns(expectedViewModel);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferTransactionDetailsQuery>()))
                .ReturnsAsync(response);

            //Act
            var result = await _controller.TransferDetail(_query);

            //Assert
            var view = result as ViewResult;

            var viewModel = view?.Model as TransferTransactionDetailsViewModel;

            Assert.AreEqual(expectedViewModel, viewModel);
        }
    }
}

using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountTransferTransactionDetails;
using SFA.DAS.EAS.Application.Queries.GetTransactionsDownloadResultViewModel;
using SFA.DAS.EAS.Application.Queries.GetTransferTransactionDetails;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Transfers;
using SFA.DAS.EAS.Web.Authentication;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Transactions;
using SFA.DAS.HashingService;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace SFA.DAS.EAS.Web.UnitTests.Controllers.EmployerAccountTransactionsControllerTests
{
    class WhenIViewTranferTransactions
    {
        private const long SenderAccountId = 1;
        private const string ReceiverPublicHashedAccountId = "DEF456";
        private const string PeriodEnd = "1718-R01";

        private Web.Controllers.EmployerAccountTransactionsController _controller;
        private Mock<EmployerAccountTransactionsOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;
        private Mock<IFeatureToggleService> _featureToggle;
        private Mock<IMultiVariantTestingService> _userViewTestingService;
        private Mock<ICookieStorageService<FlashMessageViewModel>> _flashMessage;
        private Mock<IHashingService> _hashingService;
        private Mock<IMapper> _mapper;
        private Mock<IMediator> _mediator;
        private GetTransferTransactionDetailsQuery _query;

        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
            _owinWrapper = new Mock<IAuthenticationService>();
            _featureToggle = new Mock<IFeatureToggleService>();
            _userViewTestingService = new Mock<IMultiVariantTestingService>();
            _flashMessage = new Mock<ICookieStorageService<FlashMessageViewModel>>();
            _mapper = new Mock<IMapper>();

            _hashingService = new Mock<IHashingService>();
            _mediator = new Mock<IMediator>();

            _controller = new Web.Controllers.EmployerAccountTransactionsController(_owinWrapper.Object,
                _featureToggle.Object, _hashingService.Object, _mediator.Object,
                _orchestrator.Object, _userViewTestingService.Object, _flashMessage.Object,
                Mock.Of<ITransactionFormatterFactory>(), _mapper.Object);

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
            var expectedViewModel = new TransferSenderTransactionDetailsViewModel
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

            _mapper.Setup(x => x.Map<TransferSenderTransactionDetailsViewModel>(response))
                   .Returns(expectedViewModel);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetTransferTransactionDetailsQuery>()))
                .ReturnsAsync(response);

            //Act
            var result = await _controller.TransferDetail(_query);

            //Assert
            var view = result as ViewResult;

            var viewModel = view?.Model as TransferSenderTransactionDetailsViewModel;

            Assert.AreEqual(expectedViewModel, viewModel);
        }
    }
}

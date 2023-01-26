using Moq;
using NUnit.Framework;
using SFA.DAS.Authentication;
using SFA.DAS.EmployerFinance.Models.Account;
using SFA.DAS.EmployerFinance.Models.Transaction;
using SFA.DAS.EmployerFinance.Web.Orchestrators;
using SFA.DAS.EmployerFinance.Web.ViewModels;
using System;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using SFA.DAS.NLog.Logger;
using Microsoft.AspNetCore.Mvc;

namespace SFA.DAS.EmployerFinance.Web.UnitTests.Controllers
{
    public class AccountTransactionControllerTests
    {
        private Web.Controllers.EmployerAccountTransactionsController _controller;
        private Mock<EmployerAccountTransactionsOrchestrator> _orchestrator;
        private Mock<IAuthenticationService> _owinWrapper;


        [SetUp]
        public void Arrange()
        {
            _orchestrator = new Mock<EmployerAccountTransactionsOrchestrator>();
            _owinWrapper = new Mock<IAuthenticationService>();

            _orchestrator.Setup(x => x.GetAccountTransactions(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()))
                .ReturnsAsync(new OrchestratorResponse<TransactionViewResultViewModel>
                {
                    Data = new TransactionViewResultViewModel(DateTime.Now)
                    {
                        Account = new EAS.Account.Api.Types.AccountDetailViewModel(),
                        Model = new TransactionViewModel
                        {
                            Data = new AggregationData()
                        },
                        AccountHasPreviousTransactions = true
                    }
                });

            _controller = new Web.Controllers.EmployerAccountTransactionsController(
                _owinWrapper.Object, _orchestrator.Object, Mock.Of<IMapper>(), Mock.Of<IMediator>(), Mock.Of<ILog>());
        }

        [Test]
        public async Task ThenTransactionsAreRetrievedForTheAccount()
        {
            //Act
            var result = await _controller.TransactionsView("TEST", 2017, 1);

            //Assert
            _orchestrator.Verify(x => x.GetAccountTransactions(It.Is<string>(s => s == "TEST"), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once);
            Assert.IsNotNull(result as ViewResult);
        }

        [Test]
        public async Task ThenPrevioussTransactionsStatusIsShown()
        {
            //Act
            var result = await _controller.TransactionsView("TEST", 2017, 1);

            var viewResult = result as ViewResult;
            var viewModel = viewResult?.Model as OrchestratorResponse<TransactionViewResultViewModel>;

            //Assert
            Assert.IsNotNull(viewModel);
            Assert.IsTrue(viewModel.Data.AccountHasPreviousTransactions);
        }
    }
}

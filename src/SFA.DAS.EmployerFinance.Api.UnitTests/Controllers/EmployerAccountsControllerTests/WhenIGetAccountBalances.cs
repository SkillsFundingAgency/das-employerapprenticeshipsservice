using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EmployerFinance.Api.Controllers;
using SFA.DAS.EmployerFinance.Api.Orchestrators;
using SFA.DAS.EmployerFinance.Queries.GetAccountBalances;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SFA.DAS.EmployerFinance.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public class WhenIGetAccountBalances
    {
        private EmployerAccountsController _employerAccountsController;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<IMapper> _mapper;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _mapper = new Mock<IMapper>();
            _hashingService = new Mock<IHashingService>();

            var orchestrator = new FinanceOrchestrator(_mediator.Object, _logger.Object, _mapper.Object, _hashingService.Object);
            _employerAccountsController = new EmployerAccountsController(orchestrator);

        }

        [Test]
        public async Task ThenReturnTheAccountBalance()
        {
            //Arrange            
            var accountIds = new List<long> { 1, 2 };
            var request = new BulkAccountsRequest{
                AccountIds = accountIds
            };
            var accountBalancesResponse = new GetAccountBalancesResponse
            {
                Accounts = new List<Models.Account.AccountBalance> { new Models.Account.AccountBalance { AccountId = 1, Balance = 10000 }, new Models.Account.AccountBalance {AccountId =2, Balance = 20000 } }
            };
            _mediator.Setup(x => x.SendAsync(It.Is<GetAccountBalancesRequest>(q => q.AccountIds == accountIds))).ReturnsAsync(accountBalancesResponse);

            //Act
            var response = await _employerAccountsController.Index(request);

            //Assert
            Assert.IsNotNull(response);
        }
    }
}

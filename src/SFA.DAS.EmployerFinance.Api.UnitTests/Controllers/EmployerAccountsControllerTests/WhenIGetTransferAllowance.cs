using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Api.Controllers;
using SFA.DAS.EmployerFinance.Api.Orchestrators;
using SFA.DAS.EmployerFinance.Queries.GetTransferAllowance;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EmployerFinance.Api.UnitTests.Controllers.EmployerAccountsControllerTests
{
    public class WhenIGetTransferAllowance
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
        public async Task ThenReturnTheTransferAllowance()
        {
            //Arrange
            var hashedAccountId = "ABC123";

            var accountBalancesResponse = new GetTransferAllowanceResponse
            {
                TransferAllowance = new Models.Transfers.TransferAllowance { RemainingTransferAllowance = 10 }
            };
            
            _mediator.Setup(x => x.SendAsync(It.Is<GetTransferAllowanceQuery>(q => q.AccountId == It.IsAny<long>()))).ReturnsAsync(accountBalancesResponse);

            //Act
            var response = await _employerAccountsController.GetTransferAllowance(hashedAccountId);

            //Assert
            Assert.IsNotNull(response);
        }
    }
}

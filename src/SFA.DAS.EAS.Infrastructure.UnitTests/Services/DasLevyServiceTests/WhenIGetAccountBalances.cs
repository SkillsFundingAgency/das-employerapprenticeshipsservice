using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountBalances;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.DasLevyServiceTests
{
    public class WhenIGetAccountBalances
    {
        private Mock<IMediator> _mediator;
        private DasLevyService _dasLevyService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>())).ReturnsAsync(new GetAccountBalancesResponse { Accounts = new List<AccountBalance> { new AccountBalance() } });

            _dasLevyService = new DasLevyService(_mediator.Object);
        }

        [Test]
        public async Task ThenTheMediatorMethodIsCalled()
        {
            //Act
            await _dasLevyService.GetAllAccountBalances();

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.IsAny<GetAccountBalancesRequest>()), Times.Once);
        }

        [Test]
        public async Task ThenTheResponseFromTheQueryIsReturned()
        {
            
            //Act
            var actual = await _dasLevyService.GetAllAccountBalances();

            //Assert
            Assert.IsNotEmpty(actual);
        }

        [Test]
        public async Task ThenIfNullIsReturnedFromTheResponseNullIsReturnedFromTheCall()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountBalancesRequest>())).ReturnsAsync(new GetAccountBalancesResponse { Accounts = null});

            //Act
            var actual = await _dasLevyService.GetAllAccountBalances();

            //Assert
            Assert.IsNull(actual);
        }
    }
}

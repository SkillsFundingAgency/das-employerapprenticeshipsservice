﻿using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetAccountTransactionDetail;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.DasLevyServiceTests
{
    public class WhenIGetTransactionDetailInformation
    {
        private DasLevyService _dasLevyService;
        private Mock<IMediator> _mediator;
        private long ExpectedId = 43423455;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTransactionDetailQuery>())).ReturnsAsync(new GetAccountTransactionDetailResponse() { Data= new List<TransactionLineDetail> { new TransactionLineDetail() } });

            _dasLevyService = new DasLevyService(_mediator.Object);
        }
        
        [Test]
        public async Task ThenTheMediatorMethodIsCalled()
        {
            //Act
            await _dasLevyService.GetTransactionDetailById(ExpectedId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetAccountTransactionDetailQuery>(c => c.Id.Equals(ExpectedId))), Times.Once);
        }

        [Test]
        public async Task ThenTheResponseFromTheQueryIsReturned()
        {
            //Act
            var actual = await _dasLevyService.GetTransactionDetailById(ExpectedId);

            //Assert
            Assert.IsNotEmpty(actual);
        }

        [Test]
        public async Task ThenIfNullIsReturnedFromTheResponseNullIsReturnedFromTheCall()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetAccountTransactionDetailQuery>())).ReturnsAsync(new GetAccountTransactionDetailResponse { Data = null });

            //Act
            var actual = await _dasLevyService.GetTransactionDetailById(ExpectedId);

            //Assert
            Assert.IsNull(actual);
        }

    }
}

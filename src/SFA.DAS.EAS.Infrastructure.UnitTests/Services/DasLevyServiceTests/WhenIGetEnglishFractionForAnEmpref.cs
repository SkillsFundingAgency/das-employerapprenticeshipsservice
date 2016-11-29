using System.Collections.Generic;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.AccountTransactions.GetEnglishFrationDetail;
using SFA.DAS.EAS.Domain.Models.Levy;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.DasLevyServiceTests
{
    public class WhenIGetEnglishFractionForAnEmpref
    {
        private Mock<IMediator> _mediator;
        private DasLevyService _dasLevyService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEnglishFractionDetailByEmpRefQuery>())).ReturnsAsync(new GetEnglishFractionDetailResposne() { FractionDetail = new List<DasEnglishFraction> { new DasEnglishFraction() } });

            _dasLevyService = new DasLevyService(_mediator.Object);
        }

        [Test]
        public async Task ThenTheMediatorMethodIsCalled()
        {
            //Arrange
            var empRef = "123FGV";
            var userId = "123AA";
            var accountId = "456TGF";

            //Act
            await _dasLevyService.GetEnglishFractionHistory(empRef, userId, accountId);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetEnglishFractionDetailByEmpRefQuery>(c=>c.AccountId.Equals(accountId) && c.EmpRef.Equals(empRef) && c.UserId.Equals(userId))), Times.Once);
        }

        [Test]
        public async Task ThenTheResponseFromTheQueryIsReturned()
        {
            //Arrange
            var empRef = "123FGV";
            var userId = "123AA";
            var accountId = "456TGF";

            //Act
            var actual = await _dasLevyService.GetEnglishFractionHistory(empRef, userId, accountId);

            //Assert
            Assert.IsNotEmpty(actual);
            Assert.IsAssignableFrom<List<DasEnglishFraction>>(actual);
        }
        
    }
}

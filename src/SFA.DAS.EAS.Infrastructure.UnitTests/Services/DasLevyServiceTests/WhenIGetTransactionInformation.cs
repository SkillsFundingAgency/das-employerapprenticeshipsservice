using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.DasLevyServiceTests
{
    public class WhenIGetTransactionInformation
    {
        private DasLevyService _dasLevyService;
        private Mock<IDasLevyRepository> _mediator;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IDasLevyRepository>();

            _dasLevyService = new DasLevyService(_mediator.Object);    
        }

        [Test]
        public async Task ThenTheGetTransactionLineQueryIsCalled()
        {
            //Arrange
            var expectedAccountId = 14545;

            //Act
            await _dasLevyService.GetTransactionsByAccountId(expectedAccountId);

            //Assert
            _mediator.Verify(x => x.GetTransactions(expectedAccountId));
        }


    }
}

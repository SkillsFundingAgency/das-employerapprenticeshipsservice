﻿using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Models.Paye;
using SFA.DAS.EmployerFinance.Queries.GetEmployerSchemes;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.DasAccountServiceTests
{
    public class WhenIGetEmployerInformation
    {
        private Mock<IMediator> _mediator;
        private DasAccountService _dasAccountService;
        private const long ExpectedAccountId = 656474;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerSchemesQuery>())).ReturnsAsync(new GetEmployerSchemesResponse {PayeSchemes = new PayeSchemes()});

            _dasAccountService = new DasAccountService(_mediator.Object);
        }

        [Test]
        public async Task ThenTheGetEmployerSchemesQueryIsCalledWithThePassedAccountId()
        {
            //Act
            await _dasAccountService.GetAccountSchemes(ExpectedAccountId);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<GetEmployerSchemesQuery>(c=>c.Id.Equals(ExpectedAccountId))));
        }
    }
}

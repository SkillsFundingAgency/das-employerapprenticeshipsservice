﻿using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerFinance.Commands.UpdatePayeInformation;
using SFA.DAS.EmployerFinance.Services;

namespace SFA.DAS.EmployerFinance.UnitTests.Services.DasAccountServiceTests
{
    public class WhenIUpdatePayeSchemeNames
    {
        private Mock<IMediator> _mediator;
        private DasAccountService _dasAccountService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();

            _dasAccountService = new DasAccountService(_mediator.Object);
        }

        [Test]
        public async Task ThenTheCommandIsCalledWithThePassedParameters()
        {
            //Arrange
            var expectedEmpref = "456TGB";

            //Act
            await _dasAccountService.UpdatePayeScheme(expectedEmpref);

            //Assert
            _mediator.Verify(x=>x.SendAsync(It.Is<UpdatePayeInformationCommand>(c=>c.PayeRef.Equals(expectedEmpref))));
        }
    }
}
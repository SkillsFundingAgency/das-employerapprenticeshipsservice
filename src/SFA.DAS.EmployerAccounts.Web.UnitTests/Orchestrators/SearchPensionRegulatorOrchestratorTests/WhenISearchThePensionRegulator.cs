using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;
using SFA.DAS.EmployerAccounts.Queries.GetPensionRegulator;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.SearchPensionRegulatorOrchestratorTests
{
    public class WhenISearchThePensionRegulator
    {
        private SearchPensionRegulatorOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
      
        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
          
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPensionRegulatorRequest>()))
                .ReturnsAsync(new GetPensionRegulatorResponse
                {
                    Organisations = new List<Organisation>()
                });

            _orchestrator = new SearchPensionRegulatorOrchestrator(
                _mediator.Object, 
                Mock.Of<ICookieStorageService<EmployerAccountData>>());
        }

        [Test]
        public async Task ThenTheMediatorIsCalledToGetThePensionRegulatorResult()
        {
            //Arrange
            var payeRef = "123/4567";
          
            //Act
            await _orchestrator.SearchPensionRegulator(payeRef);

            //Assert
            _mediator.Verify(x => x.SendAsync(It.Is<GetPensionRegulatorRequest>(c => c.PayeRef.Equals(payeRef))), Times.Once);
        }

        [Test]
        public async Task ThenThePensionRegulatorOrganisationListIsReturnedInTheResult()
        {          
            //Act
            var actual = await _orchestrator.SearchPensionRegulator(It.IsAny<string>());

            //Assert
            Assert.IsAssignableFrom<OrchestratorResponse<SearchPensionRegulatorResultsViewModel>>(actual);
        }

        [Test]
        public async Task ThenAnInvalidRequestExceptionIsHandledTheOrchestratorResponseIsSetToBadRequest()
        {
            //Arrange
            _mediator.Setup(x => x.SendAsync(It.IsAny<GetPensionRegulatorRequest>()))
                .ThrowsAsync(new InvalidRequestException(new Dictionary<string, string> {{"", ""}}));

            //Act
            var actual = await _orchestrator.SearchPensionRegulator(It.IsAny<string>());

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest,actual.Status);
        }     
    }
}

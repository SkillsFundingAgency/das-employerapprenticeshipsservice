using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountLegalEntities;
using SFA.DAS.EAS.Domain.Entities.Account;
using SFA.DAS.EAS.Web.Models;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    public class WhenICreateALegalEntityOfOtherType
    {
        private Web.Orchestrators.OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<IMapper> _mapper;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
            _mapper = new Mock<IMapper>();

            _orchestrator = new Web.Orchestrators.OrganisationOrchestrator(_mediator.Object, _logger.Object, _mapper.Object);
        }

        [Test]
        public async Task ThenTheNameIsMandatory()
        {
            var request = new OrganisationDetailsViewModel();
            var result = await _orchestrator.ValidateLegalEntityName(request);

            Assert.IsFalse(result.Data.Valid);
            Assert.IsTrue(result.Data.ErrorDictionary.ContainsKey("Name"));
        }

        [Test]
        public async Task ThenTheLegalEntityIsValidIfNameIsProvided()
        {
            var request = new OrganisationDetailsViewModel
            {
                Name = "Test Organisation"
            };
            var result = await _orchestrator.ValidateLegalEntityName(request);

            Assert.IsTrue(result.Data.Valid);
        }
    }
}

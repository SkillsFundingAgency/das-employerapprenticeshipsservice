using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.Account;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using SFA.DAS.HashingService;
using SFA.DAS.NLog.Logger;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    public class WhenICreateALegalEntityOfOtherType
    {
        private Web.Orchestrators.OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILog> _logger;
        private Mock<IMapper> _mapper;
        private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;
        private Mock<IHashingService> _hashingService;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILog>();
            _mapper = new Mock<IMapper>();
            _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();
            _hashingService = new Mock<IHashingService>();

            _orchestrator = new Web.Orchestrators.OrganisationOrchestrator(_mediator.Object, _logger.Object, _mapper.Object, _cookieService.Object, _hashingService.Object);
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

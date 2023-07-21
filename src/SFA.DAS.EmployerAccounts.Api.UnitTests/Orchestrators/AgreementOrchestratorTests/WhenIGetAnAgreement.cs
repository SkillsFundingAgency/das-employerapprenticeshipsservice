using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Api.Orchestrators;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;

namespace SFA.DAS.EmployerAccounts.Api.UnitTests.Orchestrators.AgreementOrchestratorTests
{
    internal class WhenIGetAnAgreement
    {
        private AgreementOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger<AgreementOrchestrator>> _logger;
        private IMapper _mapper;
        private Models.EmployerAgreement.EmployerAgreementView _agreement;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger<AgreementOrchestrator>>();
            _mapper = ConfigureMapper();
            _agreement = new Models.EmployerAgreement.EmployerAgreementView();

            var response = new GetEmployerAgreementByIdResponse()
            {
                EmployerAgreement = _agreement
            };

            _orchestrator = new AgreementOrchestrator(_mediator.Object, _logger.Object, _mapper);

            _mediator.Setup(x => x.Send(It.IsAny<GetEmployerAgreementByIdRequest>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(response);
        }

        [Test]
        public async Task ThenARequestShouldBeCreatedAndItsResponseReturned()
        {
            //Arrange
            const long agreementId = 123;

            //Act
            var result = await _orchestrator.GetAgreement(agreementId);

            //Assert
            result.Should().BeEquivalentTo(_agreement, option => option.ExcludingMissingMembers());
        }

        private static IMapper ConfigureMapper()
        {
            var profiles = Assembly.Load("SFA.DAS.EmployerAccounts.Api")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(c =>
            {
                foreach (var profile in profiles)
                {
                    c.AddProfile(profile);
                }
            });

            return config.CreateMapper();
        }
    }
}

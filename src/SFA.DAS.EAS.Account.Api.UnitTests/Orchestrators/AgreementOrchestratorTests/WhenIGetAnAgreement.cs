using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Castle.Core.Internal;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Account.Api.Orchestrators;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementById;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Account.Api.UnitTests.Orchestrators.AgreementOrchestratorTests
{
    internal class WhenIGetAnAgreement
    {
        private AgreementOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private IMapper _mapper;
        private EmployerAgreementView _agreement;

        [SetUp]
        public void Arrange()
        {
            _mediator = new Mock<IMediator>();
            _mapper = ConfigureMapper();
            _agreement = new EmployerAgreementView();

            var response = new GetEmployerAgreementByIdResponse()
            {
                EmployerAgreement = _agreement
            };

            _orchestrator = new AgreementOrchestrator(_mediator.Object, _mapper);

            _mediator.Setup(x => x.SendAsync(It.IsAny<GetEmployerAgreementByIdRequest>()))
                .ReturnsAsync(response);
        }

        [Test]
        public async Task ThenARequestShouldBeCreatedAndItsResponseReturned()
        {
            //Arrange
            const string hashedAgreementId = "ABC123";

            //Act
            var result = await _orchestrator.GetAgreement(hashedAgreementId);

            //Assert
            result.Data.ShouldBeEquivalentTo(_agreement);
        }

        private IMapper ConfigureMapper()
        {
            var profiles = Assembly.Load("SFA.DAS.EAS.Account.Api")
                .GetTypes()
                .Where(t => typeof(Profile).IsAssignableFrom(t))
                .Select(t => (Profile)Activator.CreateInstance(t));

            var config = new MapperConfiguration(c =>
            {
                profiles.ForEach(c.AddProfile);
            });

            return config.CreateMapper();
        }
    }
}

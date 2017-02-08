using System.Collections.Generic;
using System.Net;
using AutoMapper;
using MediatR;
using Moq;
using NLog;
using NUnit.Framework;
using SFA.DAS.EAS.Application;
using SFA.DAS.EAS.Application.Commands.CreateOrganisationAddress;
using SFA.DAS.EAS.Web.Orchestrators;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.Organisation;
using StructureMap.Graph.Scanning;

namespace SFA.DAS.EAS.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests
{
    class WhenIAddAnOrganisationAddress
    {
        private OrganisationOrchestrator _orchestrator;
        private Mock<IMediator> _mediator;
        private Mock<ILogger> _logger;
        private Mock<IMapper> _mapper;

        private CreateOrganisationAddressRequest _request;
        private AddOrganisationAddressViewModel _viewModel;
        private string _address;
        private Mock<ICookieService> _cookieService;

        [SetUp]
        public void Arrange()
        {
            _viewModel = new AddOrganisationAddressViewModel();

            _request = new CreateOrganisationAddressRequest()
            {
                AddressFirstLine = "123, Test Lane",
                AddressSecondLine = "Test Garden",
                TownOrCity = "Test Town",
                County = "Testshire",
                Postcode = "TE12 3ST"
            };

            _address = $"{_request.AddressFirstLine}, {_request.AddressSecondLine}, " +
                       $"{_request.TownOrCity}, {_request.County}, {_request.Postcode}";
           
            _mediator = new Mock<IMediator>();
            _logger = new Mock<ILogger>();
            _mapper = new Mock<IMapper>();

            _mapper.Setup(x => x.Map<CreateOrganisationAddressRequest>(It.IsAny<AddOrganisationAddressViewModel>()))
                .Returns(_request);

            _mediator.Setup(x => x.Send(It.IsAny<CreateOrganisationAddressRequest>()))
                     .Returns(new CreateOrganisationAddressResponse {Address = _address});

            _cookieService = new Mock<ICookieService>();
            

             _orchestrator = new OrganisationOrchestrator(_mediator.Object, _logger.Object, _mapper.Object, _cookieService.Object);
        }

        [Test]
        public void ThenIShouldReturnAnOrganisationWithTheCorrectAddress()
        {
            //Act
            var result = _orchestrator.AddOrganisationAddress(_viewModel);

            //Assert
            _mapper.Verify(x => x.Map<CreateOrganisationAddressRequest>(_viewModel.Address), Times.Once);
            _mediator.Verify(x => x.Send(_request), Times.Once);
            Assert.AreEqual(_address, result.Data.Address);
        }

        [Test]
        public void ThenIShouldReturnBadRequestIfValidationHasFailed()
        {
            //Arange
            _mediator.Setup(x => x.Send(It.IsAny<CreateOrganisationAddressRequest>()))
                .Throws(new InvalidRequestException(new Dictionary<string, string>
                    {
                        {"Test", "Test"}
                    })
                );


            //Act
            var result = _orchestrator.AddOrganisationAddress(_viewModel);

            //Assert
            Assert.AreEqual(HttpStatusCode.BadRequest, result.Status);
        }
    }
}

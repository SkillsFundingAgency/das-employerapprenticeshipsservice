using System.Collections.Generic;
using System.Net;
using System.Threading;
using AutoMapper;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Commands.CreateOrganisationAddress;
using SFA.DAS.EmployerAccounts.Exceptions;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Web.Orchestrators;
using SFA.DAS.EmployerAccounts.Web.ViewModels;

namespace SFA.DAS.EmployerAccounts.Web.UnitTests.Orchestrators.OrganisationOrchestratorTests;

class WhenIAddAnOrganisationAddress
{
    private OrganisationOrchestrator _orchestrator;
    private Mock<IMediator> _mediator;
    private Mock<IMapper> _mapper;
    private Mock<IAccountLegalEntityPublicHashingService> _hashingService;

    private CreateOrganisationAddressRequest _request;
    private AddOrganisationAddressViewModel _viewModel;
    private string _address;
    private Mock<ICookieStorageService<EmployerAccountData>> _cookieService;

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
        _mapper = new Mock<IMapper>();
        _hashingService = new Mock<IAccountLegalEntityPublicHashingService>();

        _mapper.Setup(x => x.Map<CreateOrganisationAddressRequest>(It.IsAny<AddOrganisationAddressViewModel>()))
            .Returns(_request);

        _mediator.Setup(x => x.Send(It.IsAny<CreateOrganisationAddressRequest>(), CancellationToken.None))
            .ReturnsAsync(new CreateOrganisationAddressResponse { Address = _address });

        _cookieService = new Mock<ICookieStorageService<EmployerAccountData>>();


        _orchestrator = new OrganisationOrchestrator(_mediator.Object, _mapper.Object, _cookieService.Object, _hashingService.Object);
    }

    [Test]
    public void ThenIShouldReturnAnOrganisationWithTheCorrectAddress()
    {
        //Act
        var result = _orchestrator.AddOrganisationAddress(_viewModel);

        //Assert
        _mapper.Verify(x => x.Map<CreateOrganisationAddressRequest>(_viewModel.Address), Times.Once);
        _mediator.Verify(x => x.Send(_request, CancellationToken.None), Times.Once);
        Assert.AreEqual(_address, result.Data.Address);
    }

    [Test]
    public void ThenIShouldReturnBadRequestIfValidationHasFailed()
    {
        //Arange
        _mediator.Setup(x => x.Send(It.IsAny<CreateOrganisationAddressRequest>(), CancellationToken.None))
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


    [Test]
    public void ThenTheCodeIsDefaultedToEmptyStringIfNull()
    {
        //Arrange
        _viewModel.OrganisationReferenceNumber = null;

        //Act
        var actual = _orchestrator.AddOrganisationAddress(_viewModel);

        //Assert
        Assert.AreEqual("", actual.Data.ReferenceNumber);
    }
}
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using HMRC.ESFA.Levy.Api.Types;
using MediatR;
using Microsoft.Extensions.Logging;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces.Hmrc;
using SFA.DAS.EmployerAccounts.Models.PAYE;
using SFA.DAS.EmployerAccounts.Queries.GetHmrcEmployerInformation;
using SFA.DAS.EmployerAccounts.Queries.GetPayeSchemeInUse;
using SFA.DAS.EmployerAccounts.TestCommon;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetHmrcEmployerInformationTests;

public class WhenRequestingEmployerInformation
{
    private GetHmrcEmployerInformationHandler _getHmrcEmployerInformationHandler;
    private Mock<IValidator<GetHmrcEmployerInformationQuery>> _validator;
    private Mock<IHmrcService> _hmrcService;
    private Mock<IMediator> _mediator;
    private Mock<ILogger<GetHmrcEmployerInformationHandler>> _logger;
    private const string ExpectedAuthToken = "token1";
    private const string ExpectedAuthTokenInUse = "token12";
    private const string ExpectedAuthTokenNoScheme = "token5";
    private const string ExpectedEmpref = "123/avf123";
    private const string ExpectedEmprefInUse = "456/avf123";
    private const string ExpectedEmprefAssociatedName = "test company";

    [SetUp]
    public void Arrange()
    {
        _logger = new Mock<ILogger<GetHmrcEmployerInformationHandler>>();

        _hmrcService = new Mock<IHmrcService>();
        _hmrcService.Setup(x => x.DiscoverEmpref(ExpectedAuthToken)).ReturnsAsync(ExpectedEmpref);
        _hmrcService.Setup(x => x.DiscoverEmpref(ExpectedAuthTokenInUse)).ReturnsAsync(ExpectedEmprefInUse);
        _hmrcService.Setup(x => x.DiscoverEmpref(ExpectedAuthTokenNoScheme)).ReturnsAsync(() => null);
        _hmrcService.Setup(x => x.GetEmprefInformation(ExpectedAuthToken, ExpectedEmpref)).ReturnsAsync(new EmpRefLevyInformation { Employer = new Employer { Name = new Name { EmprefAssociatedName = ExpectedEmprefAssociatedName } }, Links = new Links() });
        _hmrcService.Setup(x => x.GetEmprefInformation(ExpectedAuthTokenInUse, ExpectedEmprefInUse)).ReturnsAsync(new EmpRefLevyInformation { Employer = new Employer { Name = new Name { EmprefAssociatedName = ExpectedEmprefAssociatedName } }, Links = new Links() });

        _validator = new Mock<IValidator<GetHmrcEmployerInformationQuery>>();
        _validator.Setup(x => x.Validate(It.IsAny<GetHmrcEmployerInformationQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string>() });

        _mediator = new Mock<IMediator>();
        _mediator.Setup(x => x.Send(It.IsAny<GetPayeSchemeInUseQuery>(),It.IsAny<CancellationToken>())).ReturnsAsync(new GetPayeSchemeInUseResponse { PayeScheme = new PayeScheme { Ref = ExpectedEmprefInUse } });
        _mediator.Setup(x => x.Send(It.Is<GetPayeSchemeInUseQuery>(c => c.Empref == ExpectedEmpref), It.IsAny<CancellationToken>())).ReturnsAsync(new GetPayeSchemeInUseResponse { PayeScheme = null });
            
        _getHmrcEmployerInformationHandler = new GetHmrcEmployerInformationHandler(_validator.Object, _hmrcService.Object, _mediator.Object, _logger.Object);
    }

    [Test]
    public async Task ThenTheValidatorIsCalled()
    {
        //Act
        await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthToken }, CancellationToken.None);

        //Assert
        _validator.Verify(x => x.Validate(It.IsAny<GetHmrcEmployerInformationQuery>()), Times.Once);
    }

    [Test]
    public void ThenTheHmrcServiceIsNotCalledIfTheMessageIsNotValidAnAnInvalidRequestExceptionIsThrown()
    {
        //Arrange
        _validator.Setup(x => x.Validate(It.IsAny<GetHmrcEmployerInformationQuery>())).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

        //Act
        Assert.ThrowsAsync<InvalidRequestException>(async () => await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery(), CancellationToken.None));

        //Assert
        _hmrcService.Verify(x => x.GetEmprefInformation(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }

    [Test]
    public async Task ThenTheHmrcServiceIsCalledWithTheCorrectData()
    {
        //Act
        await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthToken }, CancellationToken.None);

        //Assert
        _hmrcService.Verify(x => x.GetEmprefInformation(ExpectedAuthToken, ExpectedEmpref), Times.Once);
    }

    [Test]
    public async Task ThenTheResponseReturnsThePopulatedHmrcEmployerInformation()
    {
        //Act
        var actual = await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthToken }, CancellationToken.None);

        //Assert
        Assert.AreEqual(ExpectedEmprefAssociatedName, actual.EmployerLevyInformation.Employer.Name.EmprefAssociatedName);
        Assert.AreEqual(ExpectedEmpref, actual.Empref);
    }

    [Test]
    public void ThenTheSuccesfulResponseIsCheckedToSeeIfItIsAlreadyRegistered()
    {
        //Act
        Assert.ThrowsAsync<ConstraintException>(async () => await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthTokenInUse }, CancellationToken.None));

        //Assert
        _mediator.Verify(x => x.Send(It.Is<GetPayeSchemeInUseQuery>(c => c.Empref.Equals(ExpectedEmprefInUse)), CancellationToken.None));
    }

    [Test]
    public void ThenTheMessageIsNotValidIfTheSchemeAlreadyExists()
    {
        //Act
        Assert.ThrowsAsync<ConstraintException>(async () => await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthTokenInUse }, CancellationToken.None));
        _logger.VerifyLogging($"PAYE scheme {ExpectedEmprefInUse} already in use.", LogLevel.Warning, Times.Once());
    }

    [Test]
    public async Task ThenTheMessageIsValidIfTheSchemeDoesNotExist()
    {
        //Arrange
        _mediator.Setup(x => x.Send(It.IsAny<GetPayeSchemeInUseQuery>(), CancellationToken.None)).ReturnsAsync(new GetPayeSchemeInUseResponse { PayeScheme = null });

        //Act
        var actual = await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthToken }, CancellationToken.None);

        //Assert
        Assert.IsNotNull(actual);
    }

    [Test]
    public void ThenAnNotFoundExceptionIsRetrunedIfTheEmprefIsEmpty()
    {
        //Act Assert
        Assert.ThrowsAsync<NotFoundException>(async () => await _getHmrcEmployerInformationHandler.Handle(new GetHmrcEmployerInformationQuery { AuthToken = ExpectedAuthTokenNoScheme }, CancellationToken.None));
    }
}
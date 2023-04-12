using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.PensionRegulator;
using SFA.DAS.EmployerAccounts.Queries.GetOrganisationsByAorn;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetOrganisationsByAornTests;

public class WhenISearchPensionRegulator
{
    private Mock<IPensionRegulatorService> _pensionRegulatorService;
    private GetOrganisationsByAornRequest _query;
    private GetOrganisationsByAornQueryHandler _requestHandler;
    private Mock<IValidator<GetOrganisationsByAornRequest>> _requestValidator;

    [SetUp]
    public void Arrange()
    {
        _pensionRegulatorService = new Mock<IPensionRegulatorService>();
        _requestValidator = new Mock<IValidator<GetOrganisationsByAornRequest>>();
        _requestValidator.Setup(x => x.Validate(It.IsAny<GetOrganisationsByAornRequest>())).Returns(new ValidationResult());

        _query = new GetOrganisationsByAornRequest("AORN", "123/4567");

        _requestHandler = new GetOrganisationsByAornQueryHandler(_requestValidator.Object, _pensionRegulatorService.Object);
    }

    [Test]
    public void ThenIfISupplyInvalidDetailsAnErrorIsReturned()
    {
        //Arrange
        _requestValidator.Setup(x => x.Validate(_query)).Returns(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "", "" } } });

        //Act
        Assert.ThrowsAsync<InvalidRequestException>(async () => await _requestHandler.Handle(_query, CancellationToken.None));
    }

    [Test]
    public async Task ThenIfSupplyValidDetailsTheMatchingOrganisationsAreReturned()
    {
        //Arrange
        var expectedResponse = new List<Organisation>();
        _pensionRegulatorService.Setup(x => x.GetOrganisationsByAorn(_query.Aorn, _query.PayeRef)).ReturnsAsync(expectedResponse);

        //Act
        var actual = await _requestHandler.Handle(_query, CancellationToken.None);

        //Assert
        Assert.AreSame(expectedResponse, actual.Organisations);
    }
}
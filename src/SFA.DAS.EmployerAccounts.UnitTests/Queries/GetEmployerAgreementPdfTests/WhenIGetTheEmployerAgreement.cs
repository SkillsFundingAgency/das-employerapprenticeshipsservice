using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAgreementPdfTests;

public class WhenIGetTheEmployerAgreement : QueryBaseTest<GetEmployerAgreementPdfQueryHandler, GetEmployerAgreementPdfRequest, GetEmployerAgreementPdfResponse>
{
    private Mock<IPdfService> _pdfService;
    private Mock<EmployerAccountsDbContext> _db;

    public override GetEmployerAgreementPdfRequest Query { get; set; }
    public override GetEmployerAgreementPdfQueryHandler RequestHandler { get; set; }
    public override Mock<IValidator<GetEmployerAgreementPdfRequest>> RequestValidator { get; set; }

    private const long ExpectedEmployerAgreementId = 12344241;
    private const string ExpectedAgreementFileName = "FileTemplate";

    [SetUp]
    public void Arrange()
    {
        SetUp();

        Query = new GetEmployerAgreementPdfRequest { AccountId = 12321, LegalAgreementId = ExpectedEmployerAgreementId, UserId = "1234RFV" };

        _db = new Mock<EmployerAccountsDbContext>();
        var employerAgreement = new EmployerAgreement { Id = ExpectedEmployerAgreementId, Template = new AgreementTemplate { PartialViewName = ExpectedAgreementFileName } };

        var mockDbSet = new List<EmployerAgreement> { employerAgreement }.AsQueryable().BuildMockDbSet();

        _db.Setup(d => d.Agreements).Returns(mockDbSet.Object);

        _pdfService = new Mock<IPdfService>();
        _pdfService.Setup(x => x.SubstituteValuesForPdf(It.IsAny<string>())).ReturnsAsync(new MemoryStream());


        RequestHandler = new GetEmployerAgreementPdfQueryHandler(RequestValidator.Object, _pdfService.Object, new Lazy<EmployerAccountsDbContext>(() => _db.Object));
    }

    [Test]
    public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
    {

        //Act
        await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        _pdfService.Verify(x => x.SubstituteValuesForPdf($"{ExpectedAgreementFileName}.pdf"));
    }

    [Test]
    public void ThenWhenTheValidationReturnsNotAuthorizedThenAnUnauthoriedAccessExceptionIsThrown()
    {
        //Arrange
        RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAgreementPdfRequest>())).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

        //Act Assert
        Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(Query, CancellationToken.None));
    }

    [Test]
    public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
    {
        //Act
        var actual = await RequestHandler.Handle(Query, CancellationToken.None);

        //Assert
        Assert.IsAssignableFrom<GetEmployerAgreementPdfResponse>(actual);
        Assert.IsNotNull(actual.FileStream);

    }
}
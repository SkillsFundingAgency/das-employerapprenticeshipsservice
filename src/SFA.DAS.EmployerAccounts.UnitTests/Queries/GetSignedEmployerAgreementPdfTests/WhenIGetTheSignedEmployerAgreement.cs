using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetSignedEmployerAgreementPdf;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetSignedEmployerAgreementPdfTests
{
    public class WhenIGetTheSignedEmployerAgreement : QueryBaseTest<GetSignedEmployerAgreementPdfQueryHandler, GetSignedEmployerAgreementPdfRequest, GetSignedEmployerAgreementPdfResponse>
    {
        private Mock<IPdfService> _pdfService;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        public override GetSignedEmployerAgreementPdfRequest Query { get; set; }
        public override GetSignedEmployerAgreementPdfQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetSignedEmployerAgreementPdfRequest>> RequestValidator { get; set; }

        private const string ExpectedHashedLegalAgreementId = "123RFT";
        private const string ExpectedLegalAgreementTemplateName = "Agreement_1";
        private const long ExpectedLegalAgreementId = 43678;
        private const string ExpectedSignedByName = "Mr Tester";
        private const string ExpectedLegalEntityName = "Test Entity";
        private const string ExpectedLegalEntityAddress = "Test Address";
        private readonly DateTime _expectedSignedDate = new DateTime(2016, 01, 01);
        private EmployerAgreementView _employerAgreementView;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _pdfService = new Mock<IPdfService>();
            _pdfService.Setup(
                x => x.SubstituteValuesForPdf($"{ExpectedLegalAgreementTemplateName}_Sub.pdf", It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(new MemoryStream());

            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();

            _employerAgreementView = new EmployerAgreementView
            {
                Status = EmployerAgreementStatus.Signed,
                TemplatePartialViewName = ExpectedLegalAgreementTemplateName,
                SignedByName = ExpectedSignedByName,
                SignedDate = _expectedSignedDate,
                LegalEntityName = ExpectedLegalEntityName,
                LegalEntityAddress = ExpectedLegalEntityAddress
            };

            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedLegalAgreementId))
                .ReturnsAsync(_employerAgreementView);

            Query = new GetSignedEmployerAgreementPdfRequest
            {
                AccountId = 1234,
                LegalAgreementId = ExpectedLegalAgreementId,
                UserId = "12345RFV"
            };

            RequestHandler = new GetSignedEmployerAgreementPdfQueryHandler(RequestValidator.Object, _pdfService.Object, _employerAgreementRepository.Object);

        }

        [Test]
        public void ThenIfTheValidatorReturnsUnAuthorizedThenAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetSignedEmployerAgreementPdfRequest>())).ReturnsAsync(new ValidationResult { IsUnauthorized = true, ValidationDictionary = new Dictionary<string, string>() });

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetSignedEmployerAgreementPdfRequest(), CancellationToken.None));

        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _employerAgreementRepository.Verify(x => x.GetEmployerAgreement(ExpectedLegalAgreementId), Times.Once);
            _pdfService.Verify(x => x.SubstituteValuesForPdf($"{ExpectedLegalAgreementTemplateName}_Sub.pdf", It.Is<Dictionary<string, string>>(
                                                                                                c => c.ContainsValue(ExpectedSignedByName)
                                                                                            && c.ContainsValue(ExpectedLegalEntityName)
                                                                                            && c.ContainsValue(_expectedSignedDate.ToString("d MMMM yyyy"))
                                                                                            && c.ContainsValue(ExpectedLegalEntityAddress))));

        }

        [Test]
        public async Task ThenTheAddressIsAlwaysPopulatedForFiveLines()
        {
            //Arrange
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedLegalAgreementId))
                .ReturnsAsync(new EmployerAgreementView
                {
                    Status = EmployerAgreementStatus.Signed,
                    TemplatePartialViewName = ExpectedLegalAgreementTemplateName,
                    SignedByName = ExpectedSignedByName,
                    SignedDate = _expectedSignedDate,
                    LegalEntityName = ExpectedLegalEntityName,
                    LegalEntityAddress = "Test1,Test"
                });

            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _pdfService.Verify(
                x =>
                    x.SubstituteValuesForPdf($"{ExpectedLegalAgreementTemplateName}_Sub.pdf",
                        It.Is<Dictionary<string, string>>(
                            c => c.ContainsKey("LegalEntityAddress_0")
                            && c.ContainsKey("LegalEntityAddress_1")
                            && c.ContainsKey("LegalEntityAddress_2")
                            && c.ContainsKey("LegalEntityAddress_3")
                            && c.ContainsKey("LegalEntityAddress_4")
                            )));
        }

        [TestCase(EmployerAgreementStatus.Pending)]
        [TestCase(EmployerAgreementStatus.Removed)]
        public void ThenIfTheAgreementIsNotSignedThenAnErrorIsReturned(EmployerAgreementStatus status)
        {
            //Arrange
            _employerAgreementView.Status = status;

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await RequestHandler.Handle(Query, CancellationToken.None));
        }

        [Test]
        public void ThenIfAgreementSignedDateIsNotSet_ErrorReturned()
        {
            // Arrange
            _employerAgreementView.SignedDate = null;

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await RequestHandler.Handle(Query, CancellationToken.None));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.IsNotNull(actual.FileStream);
        }

        [TestCase(EmployerAgreementStatus.Superseded)]
        [TestCase(EmployerAgreementStatus.Expired)]
        public async Task ThenIfAgreementIsValidStatus_TheValueIsReturnedInTheResponse(EmployerAgreementStatus status)
        {
            // Arrange
            _employerAgreementView.Status = status;

            //Act
            var actual = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.IsNotNull(actual.FileStream);
        }
    }
}

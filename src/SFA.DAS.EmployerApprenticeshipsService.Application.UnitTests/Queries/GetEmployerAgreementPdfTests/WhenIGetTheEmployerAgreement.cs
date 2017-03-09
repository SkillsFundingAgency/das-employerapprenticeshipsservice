using System;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAgreementPdfTests
{
    public class WhenIGetTheEmployerAgreement : QueryBaseTest<GetEmployerAgreementPdfQueryHandler, GetEmployerAgreementPdfRequest, GetEmployerAgreementPdfResponse>
    {
        private Mock<IPdfService> _pdfService;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private Mock<IHashingService> _hashingService;
        public override GetEmployerAgreementPdfRequest Query { get; set; }
        public override GetEmployerAgreementPdfQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAgreementPdfRequest>> RequestValidator { get; set; }

        private const long ExpectedEmployerAgreementId = 12344241;
        private const string ExpectedAgreementFileName = "FileTemplate";

        [SetUp]
        public void Arrange()
        {
            SetUp();

            Query = new GetEmployerAgreementPdfRequest {HashedAccountId = "123RED", HashedLegalAgreementId = "668YUT",UserId = "1234RFV"};

            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedEmployerAgreementId))
                .ReturnsAsync(new EmployerAgreementView {TemplatePartialViewName = ExpectedAgreementFileName });

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue("668YUT")).Returns(ExpectedEmployerAgreementId);

            _pdfService = new Mock<IPdfService>();
            _pdfService.Setup(x => x.SubsituteValuesForPdf(It.IsAny<string>())).ReturnsAsync(new MemoryStream());

            RequestHandler = new GetEmployerAgreementPdfQueryHandler(RequestValidator.Object, _pdfService.Object, _employerAgreementRepository.Object, _hashingService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _pdfService.Verify(x=>x.SubsituteValuesForPdf($"{ExpectedAgreementFileName}.pdf"));
        }

        [Test]
        public void ThenWhenTheValidationReturnsNotAuthorizedThenAnUnauthoriedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAgreementPdfRequest>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(Query));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsAssignableFrom<GetEmployerAgreementPdfResponse>(actual);
            Assert.IsNotNull(actual.FileStream);

        }
    }
}

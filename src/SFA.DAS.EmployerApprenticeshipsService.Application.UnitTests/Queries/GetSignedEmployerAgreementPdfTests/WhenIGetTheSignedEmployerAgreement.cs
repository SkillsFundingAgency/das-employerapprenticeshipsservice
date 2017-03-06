using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetSignedEmployerAgreementPdfTests
{
    public class WhenIGetTheSignedEmployerAgreement : QueryBaseTest<GetSignedEmployerAgreementPdfQueryHandler, GetSignedEmployerAgreementPdfRequest, GetSignedEmployerAgreementPdfResponse>
    {
        private Mock<IPdfService> _pdfService;
        public override GetSignedEmployerAgreementPdfRequest Query { get; set; }
        public override GetSignedEmployerAgreementPdfQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetSignedEmployerAgreementPdfRequest>> RequestValidator { get; set; }
        
        [SetUp]
        public void Arrange()
        {
            SetUp();

            _pdfService = new Mock<IPdfService>();

            RequestHandler = new GetSignedEmployerAgreementPdfQueryHandler(RequestValidator.Object, _pdfService.Object);

        }

        [Test]
        public void ThenIfTheValidatorReturnsUnAuthorizedThenAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetSignedEmployerAgreementPdfRequest>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true,ValidationDictionary = new Dictionary<string, string>()});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetSignedEmployerAgreementPdfRequest()));

        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            throw new NotImplementedException();
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            throw new NotImplementedException();
        }
    }
}

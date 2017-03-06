using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementPdf;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAgreementPdfTests
{
    public class WhenIGetTheEmployerAgreement : QueryBaseTest<GetEmployerAgreementPdfQueryHandler, GetEmployerAgreementPdfRequest, GetEmployerAgreementPdfResponse>
    {
        private Mock<IPdfService> _pdfService;
        public override GetEmployerAgreementPdfRequest Query { get; set; }
        public override GetEmployerAgreementPdfQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAgreementPdfRequest>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            SetUp();

            Query = new GetEmployerAgreementPdfRequest {AgreementFileName = "TestFile.pdf"};

            _pdfService = new Mock<IPdfService>();
            _pdfService.Setup(x => x.SubsituteValuesForPdf(It.IsAny<string>())).ReturnsAsync(new MemoryStream());

            RequestHandler = new GetEmployerAgreementPdfQueryHandler(RequestValidator.Object, _pdfService.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {

            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _pdfService.Verify(x=>x.SubsituteValuesForPdf($"{Query.AgreementFileName}.pdf"));
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

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetSignedEmployerAgreementPdf;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetSignedEmployerAgreementPdfTests
{
    public class WhenIGetTheSignedEmployerAgreement : QueryBaseTest<GetSignedEmployerAgreementPdfQueryHandler, GetSignedEmployerAgreementPdfRequest, GetSignedEmployerAgreementPdfResponse>
    {
        private Mock<IPdfService> _pdfService;
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private Mock<IHashingService> _hashingService;
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

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _pdfService = new Mock<IPdfService>();
            _pdfService.Setup(
                x => x.SubsituteValuesForPdf(ExpectedLegalAgreementTemplateName, It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(new MemoryStream());

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedLegalAgreementId)).Returns(ExpectedLegalAgreementId);

            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedLegalAgreementId))
                .ReturnsAsync(new EmployerAgreementView
                {
                    Status = EmployerAgreementStatus.Signed,
                    TemplatePartialViewName = ExpectedLegalAgreementTemplateName,
                    SignedByName = ExpectedSignedByName,
                    SignedDate = _expectedSignedDate,
                    LegalEntityName = ExpectedLegalEntityName,
                    LegalEntityAddress = ExpectedLegalEntityAddress
                });

            RequestHandler = new GetSignedEmployerAgreementPdfQueryHandler(RequestValidator.Object, _pdfService.Object, _employerAgreementRepository.Object, _hashingService.Object);

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
            //Act
            await RequestHandler.Handle(new GetSignedEmployerAgreementPdfRequest
                {
                    HashedAccountId = "1234RFV",
                    HashedLegalAgreementId = ExpectedHashedLegalAgreementId,
                    UserId = "12345RFV"
                });

            //Assert
            _hashingService.Verify(x=>x.DecodeValue(ExpectedHashedLegalAgreementId), Times.Once);
            _employerAgreementRepository.Verify(x => x.GetEmployerAgreement(ExpectedLegalAgreementId), Times.Once);
            _pdfService.Verify(x=>x.SubsituteValuesForPdf(ExpectedLegalAgreementTemplateName,It.Is<Dictionary<string,string>>(
                                                                                                c=>c.ContainsValue(ExpectedSignedByName) 
                                                                                            && c.ContainsValue(ExpectedLegalEntityName) 
                                                                                            && c.ContainsValue(_expectedSignedDate.ToLongDateString()) 
                                                                                            && c.ContainsValue(ExpectedLegalEntityAddress))));
            
        }

        [TestCase(EmployerAgreementStatus.Pending)]
        [TestCase(EmployerAgreementStatus.Expired)]
        [TestCase(EmployerAgreementStatus.Superseded)]
        public void ThenIfTheAgreementIsNotSignedThenAnErrorIsReturned(EmployerAgreementStatus status)
        {
            //Arrange
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedLegalAgreementId))
                .ReturnsAsync(new EmployerAgreementView { Status = status });

            //Act Assert
            Assert.ThrowsAsync<InvalidRequestException>(async () => await RequestHandler.Handle(new GetSignedEmployerAgreementPdfRequest
            {
                HashedAccountId = "1234RFV",
                HashedLegalAgreementId = ExpectedHashedLegalAgreementId,
                UserId = "12345RFV"
            }));

            
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(new GetSignedEmployerAgreementPdfRequest
            {
                HashedAccountId = "1234RFV",
                HashedLegalAgreementId = ExpectedHashedLegalAgreementId,
                UserId = "12345RFV"
            });

            //Assert
            Assert.IsNotNull(actual.FileStream);
        }
    }
}

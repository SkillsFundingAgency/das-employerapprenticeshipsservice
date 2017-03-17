using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreementById;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAgreementByIdTests
{
    internal class WhenIGetAnAgreement : QueryBaseTest<GetEmployerAgreementByIdRequestHandler, GetEmployerAgreementByIdRequest, GetEmployerAgreementByIdResponse>
    {
        private static long AgreementId => 12;

        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private Mock<IHashingService> _hashingService;
        private EmployerAgreementView _agreement;

        public override GetEmployerAgreementByIdRequest Query { get; set; }
        public override GetEmployerAgreementByIdRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAgreementByIdRequest>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _hashingService = new Mock<IHashingService>();
            _agreement = new EmployerAgreementView();

            RequestHandler = new GetEmployerAgreementByIdRequestHandler(
                _employerAgreementRepository.Object,
                _hashingService.Object,
                RequestValidator.Object);

            Query = new GetEmployerAgreementByIdRequest
            {
                HashedAgreementId = "ABC123"
            };

            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(It.IsAny<long>()))
                                        .ReturnsAsync(_agreement);

            _hashingService.Setup(x => x.DecodeValue(It.IsAny<string>()))
                           .Returns(AgreementId);
        }
      
        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _employerAgreementRepository.Verify(x => x.GetEmployerAgreement(AgreementId), Times.Once);
            _hashingService.Verify(x => x.DecodeValue(Query.HashedAgreementId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual(_agreement, response.EmployerAgreement);
        }

        [Test]
        public void ThenShouldThrowExceptionIfNoAgreementFound()
        {
            //Arrange
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(It.IsAny<long>()))
                .ReturnsAsync(null);

            //Assert
            Assert.ThrowsAsync<InvalidRequestException>(() => RequestHandler.Handle(Query));
        }
    }
}

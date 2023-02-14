using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Common.Domain.Types;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetEmployerAgreementById;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetEmployerAgreementByIdTests
{
    internal class WhenIGetAnAgreement : QueryBaseTest<GetEmployerAgreementByIdRequestHandler, GetEmployerAgreementByIdRequest, GetEmployerAgreementByIdResponse>
    {
        private static long AgreementId => 12;

        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private EmployerAgreementView _agreement;

        public override GetEmployerAgreementByIdRequest Query { get; set; }
        public override GetEmployerAgreementByIdRequestHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAgreementByIdRequest>> RequestValidator { get; set; }

        [SetUp]
        public void Arrange()
        {
            base.SetUp();

            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _agreement = new EmployerAgreementView()
            {
                AgreementType = AgreementType.NonLevyExpressionOfInterest
            };

            RequestHandler = new GetEmployerAgreementByIdRequestHandler(_employerAgreementRepository.Object, RequestValidator.Object);

            Query = new GetEmployerAgreementByIdRequest
            {
                AgreementId = 121,
            };

            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(It.IsAny<long>()))
                                        .ReturnsAsync(_agreement);
        }
      
        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _employerAgreementRepository.Verify(x => x.GetEmployerAgreement(Query.AgreementId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var response = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.AreEqual(_agreement, response.EmployerAgreement);
        }

        [Test]
        public void ThenShouldThrowExceptionIfNoAgreementFound()
        {
            //Arrange
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(It.IsAny<long>()))
                .ReturnsAsync(() => null);

            //Assert
            Assert.ThrowsAsync<InvalidRequestException>(() => RequestHandler.Handle(Query, CancellationToken.None));
        }

        [Test]
        public async Task ThenIfTheMessageIsValidTheAgreementTypeIsSet()
        {
            //Act
            var response = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.AreEqual(_agreement.AgreementType, response.EmployerAgreement.AgreementType);
        }
    }
}

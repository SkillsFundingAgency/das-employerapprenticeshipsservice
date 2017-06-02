using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementRemove;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountEmployerAgreementRemove
{
    public class WhenIGetAccountEmployerAgreementToRemove : QueryBaseTest<GetAccountEmployerAgreementRemoveQueryHandler, GetAccountEmployerAgreementRemoveRequest, GetAccountEmployerAgreementRemoveResponse>
    {
        private Mock<IEmployerAgreementRepository> _repository;
        private Mock<IHashingService> _hashingService;
        public override GetAccountEmployerAgreementRemoveRequest Query { get; set; }
        public override GetAccountEmployerAgreementRemoveQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountEmployerAgreementRemoveRequest>> RequestValidator { get; set; }

        private const string ExpectedHashedAccountId = "345ASD";
        private const string ExpectedHashedAgreementId = "PHF78";
        private const long ExpectedAgreementId = 12345555;
        private const string ExpectedUserId = "098GHY";
        private const string ExpectedAgreementName = "Test Company";

        [SetUp]
        public void Arrange()
        {
            SetUp();    

            Query = new GetAccountEmployerAgreementRemoveRequest {HashedAccountId = ExpectedHashedAccountId, HashedAgreementId = ExpectedHashedAgreementId, UserId = ExpectedUserId};
            
            _repository = new Mock<IEmployerAgreementRepository>();
            _repository.Setup(x => x.GetEmployerAgreement(ExpectedAgreementId))
                .ReturnsAsync(new EmployerAgreementView
                {
                    Id = ExpectedAgreementId,
                    LegalEntityName = ExpectedAgreementName
                });

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedAgreementId)).Returns(ExpectedAgreementId);

            RequestHandler = new GetAccountEmployerAgreementRemoveQueryHandler(RequestValidator.Object, _repository.Object, _hashingService.Object);
        }

        [Test]
        public void ThenIfTheValidationResultIsUnauthorizedThenAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountEmployerAgreementRemoveRequest>())).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetAccountEmployerAgreementRemoveRequest()));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x=>x.GetEmployerAgreement(ExpectedAgreementId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual.Agreement);
            Assert.AreEqual(ExpectedAgreementId, actual.Agreement.Id);
            Assert.IsNotNull(ExpectedAgreementName,actual.Agreement.Name);
        }
    }
}

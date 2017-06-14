using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreementsRemove;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountEmployerAgreementsRemove
{
    public class WhenIGetAccountEmployerAgreementsToRemove : QueryBaseTest<GetAccountEmployerAgreementsRemoveQueryHandler,GetAccountEmployerAgreementsRemoveRequest, GetAccountEmployerAgreementsRemoveResponse>
    {
        private Mock<IEmployerAgreementRepository> _repository;
        private Mock<IHashingService> _hashingService;
        private Mock<IEmployerCommitmentApi> _commitmentsApi;
        public override GetAccountEmployerAgreementsRemoveRequest Query { get; set; }
        public override GetAccountEmployerAgreementsRemoveQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountEmployerAgreementsRemoveRequest>> RequestValidator { get; set; }

        private const string ExpectedUserId = "456TGFD";
        private const string ExpectedHashedAccountId = "456TGFD";
        private const string ExpectedLegalEntityCode = "98TYG123";
        private const long ExpectedAccountId = 98172938;
        private const long ExpectedAgreementId = 8765;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            Query = new GetAccountEmployerAgreementsRemoveRequest {HashedAccountId = ExpectedHashedAccountId, UserId = ExpectedUserId};

            _repository = new Mock<IEmployerAgreementRepository>();
            _repository.Setup(x => x.GetEmployerAgreementsToRemove(ExpectedAccountId))
                .ReturnsAsync(new List<RemoveEmployerAgreementView>
                {
                    new RemoveEmployerAgreementView {Name = "test company", CanBeRemoved = true, Id = ExpectedAgreementId,LegalEntityCode = ExpectedLegalEntityCode}   
                });

            _commitmentsApi = new Mock<IEmployerCommitmentApi>();
            _commitmentsApi.Setup(x => x.GetEmployerAccountSummary(ExpectedAccountId)).ReturnsAsync(new List<ApprenticeshipStatusSummary> {new ApprenticeshipStatusSummary()});

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedAccountId)).Returns(ExpectedAccountId);
            _hashingService.Setup(x => x.HashValue(ExpectedAgreementId)).Returns("45TGBF");

            RequestHandler = new GetAccountEmployerAgreementsRemoveQueryHandler(RequestValidator.Object, _repository.Object,_hashingService.Object, _commitmentsApi.Object);
        }

        [Test]
        public void ThenIfTheValidationResultIsUnauthorizedThenAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountEmployerAgreementsRemoveRequest>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetAccountEmployerAgreementsRemoveRequest()));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x=>x.GetEmployerAgreementsToRemove(ExpectedAccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsTrue(actual.Agreements.Any());
        }

        [Test]
        public async Task ThenIfThereIsOnlyOneResultReturnedItsCanBeRemovedValueIsSetToFalse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsFalse(actual.Agreements.First().CanBeRemoved);
        }


        [Test]
        public async Task ThenIfThereIsOnlyOneResultReturnedTheApiIsNotCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _commitmentsApi.Verify(x=>x.GetEmployerAccountSummary(It.IsAny<long>()),Times.Never);
        }

        [Test]
        public async Task ThenIfTheAgreementIsSignedThenTheApiIsCheckedForActiveCommitments()
        {
            //Arrange
            _commitmentsApi.Setup(x => x.GetEmployerAccountSummary(ExpectedAccountId))
                .ReturnsAsync(new List<ApprenticeshipStatusSummary> { new ApprenticeshipStatusSummary {LegalEntityIdentifier = ExpectedLegalEntityCode,ActiveCount = 1} });
            _repository.Setup(x => x.GetEmployerAgreementsToRemove(ExpectedAccountId))
                .ReturnsAsync(new List<RemoveEmployerAgreementView>
                {
                    new RemoveEmployerAgreementView {Name = "test company", Status = EmployerAgreementStatus.Pending, Id = ExpectedAgreementId,LegalEntityCode = "Another Code"},
                    new RemoveEmployerAgreementView {Name = "test company", Status = EmployerAgreementStatus.Signed, Id = ExpectedAgreementId,LegalEntityCode = ExpectedLegalEntityCode}
                });

            //Act
            var actual = await RequestHandler.Handle(Query);
            Assert.AreEqual(1, actual.Agreements.Count(c=>c.CanBeRemoved));
            _commitmentsApi.Verify(x => x.GetEmployerAccountSummary(ExpectedAccountId), Times.Once);
        }

        [Test]
        public async Task ThenIfTheAgreementIsSignedAndNoActiceCommitmentsTheAgreementCanBeRemoved()
        {
            //Arrange
            _commitmentsApi.Setup(x => x.GetEmployerAccountSummary(ExpectedAccountId))
                .ReturnsAsync(new List<ApprenticeshipStatusSummary> { new ApprenticeshipStatusSummary { LegalEntityIdentifier = ExpectedLegalEntityCode, CompletedCount = 1 } });
            _repository.Setup(x => x.GetEmployerAgreementsToRemove(ExpectedAccountId))
                .ReturnsAsync(new List<RemoveEmployerAgreementView>
                {
                    new RemoveEmployerAgreementView {Name = "test company", Status = EmployerAgreementStatus.Pending, Id = ExpectedAgreementId,LegalEntityCode = "Another Code"},
                    new RemoveEmployerAgreementView {Name = "test company", Status = EmployerAgreementStatus.Signed, Id = ExpectedAgreementId,LegalEntityCode = ExpectedLegalEntityCode}
                });

            //Act
            var actual = await RequestHandler.Handle(Query);
            Assert.IsTrue(actual.Agreements.All(c => c.CanBeRemoved));
        }

        [Test]
        public async Task ThenTheAgreementIsHashedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual.Agreements.First().HashedAgreementId);
            Assert.IsNotEmpty(actual.Agreements.First().HashedAgreementId);
        }
    }
}

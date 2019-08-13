

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Features;
using SFA.DAS.EmployerAccounts.Models.EmployerAgreement;
using SFA.DAS.EmployerAccounts.Models.Recruit;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementsRemove;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountEmployerAgreementsRemove
{
    public class WhenIGetAccountEmployerAgreementsToRemove : QueryBaseTest<GetAccountEmployerAgreementsRemoveQueryHandler,GetAccountEmployerAgreementsRemoveRequest, GetAccountEmployerAgreementsRemoveResponse>
    {
        private Mock<IDasRecruitService> _dasRecruitService;
        private Mock<IEmployerAgreementRepository> _repository;
        private Mock<IHashingService> _hashingService;
        private Mock<IEmployerCommitmentApi> _commitmentsApi;
        public override GetAccountEmployerAgreementsRemoveRequest Query { get; set; }
        public override GetAccountEmployerAgreementsRemoveQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountEmployerAgreementsRemoveRequest>> RequestValidator { get; set; }
        Vacancy Vacancy { get; set; }
        IEnumerable<Vacancy> Vacancies { get; set; }
        private const string ExpectedUserId = "456TGFD";
        private const string ExpectedHashedAccountId = "456TGFD";
        private const string ExpectedHashedAgreementId = "456TGED";
        private const string ExpectedLegalEntityCode = "98TYG123";
        private const long ExpectedLegalId = 3846;
        private const long ExpectedAccountId = 98172938;
        private const long ExpectedAgreementId = 8765;
        private const long ExpectedAccountLegalEntityId = 2017;
        private const string ExpectedLegalEntityName = "Hogwarts";

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
            _repository.Setup(r => r.GetEmployerAgreement(ExpectedAgreementId))
                .ReturnsAsync(new EmployerAgreementView
                {
                    AccountLegalEntityId = ExpectedAccountLegalEntityId,
                    LegalEntityId = ExpectedLegalId,
                    LegalEntityName = ExpectedLegalEntityName,
                    Status = EmployerAgreementStatus.Signed
                });
            _commitmentsApi = new Mock<IEmployerCommitmentApi>();
            _commitmentsApi.Setup(x => x.GetEmployerAccountSummary(ExpectedAccountId))
                .ReturnsAsync(new List<ApprenticeshipStatusSummary> {new ApprenticeshipStatusSummary()});

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedAccountId)).Returns(ExpectedAccountId);
            _hashingService.Setup(x => x.HashValue(ExpectedAgreementId)).Returns("45TGBF");

            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedAgreementId)).Returns(ExpectedAgreementId);
            _hashingService.Setup(x => x.HashValue(ExpectedAgreementId)).Returns(ExpectedHashedAgreementId);

            _dasRecruitService = new Mock<IDasRecruitService>();
            
            RequestHandler = new GetAccountEmployerAgreementsRemoveQueryHandler(RequestValidator.Object, _repository.Object,_hashingService.Object, _commitmentsApi.Object, _dasRecruitService.Object);
        }

        [Test]
        public void ThenIfTheValidationResultIsUnauthorizedThenAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            Vacancies = Enumerable.Empty<Vacancy>();
            _dasRecruitService.Setup(x => x.GetVacanciesByLegalEntity(ExpectedHashedAccountId, ExpectedLegalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacanciesSummary(Vacancies, 0, 0, 0, 0));
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountEmployerAgreementsRemoveRequest>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetAccountEmployerAgreementsRemoveRequest()));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Arrange
            Vacancies = Enumerable.Empty<Vacancy>();
            _dasRecruitService.Setup(x => x.GetVacanciesByLegalEntity(ExpectedHashedAccountId, ExpectedLegalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacanciesSummary(Vacancies, 0, 0, 0, 0));
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x=>x.GetEmployerAgreementsToRemove(ExpectedAccountId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Arrange
            Vacancies = Enumerable.Empty<Vacancy>();
            _dasRecruitService.Setup(x => x.GetVacanciesByLegalEntity(ExpectedHashedAccountId, ExpectedLegalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacanciesSummary(Vacancies, 0, 0, 0, 0));
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsTrue(actual.Agreements.Any());
        }

        [Test]
        public async Task ThenIfThereIsOnlyOneResultReturnedItsCanBeRemovedValueIsSetToFalse()
        {
            //Arrange
            Vacancies = Enumerable.Empty<Vacancy>();
            _dasRecruitService.Setup(x => x.GetVacanciesByLegalEntity(ExpectedHashedAccountId, ExpectedLegalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacanciesSummary(Vacancies, 0, 0, 0, 0));
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsFalse(actual.Agreements.First().CanBeRemoved);
        }


        [Test]
        public async Task ThenIfThereIsOnlyOneResultReturnedTheApiIsNotCalled()
        {
            //Arrange
            Vacancies = Enumerable.Empty<Vacancy>();
            _dasRecruitService.Setup(x => x.GetVacanciesByLegalEntity(ExpectedHashedAccountId, ExpectedLegalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacanciesSummary(Vacancies, 0, 0, 0, 0));
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _commitmentsApi.Verify(x=>x.GetEmployerAccountSummary(It.IsAny<long>()),Times.Never);
        }

        [Test]
        public async Task ThenIfTheAgreementIsSignedThenTheApiIsCheckedForActiveCommitments()
        {
            //Arrange
            Vacancies = Enumerable.Empty<Vacancy>();
            _dasRecruitService.Setup(x => x.GetVacanciesByLegalEntity(ExpectedHashedAccountId, ExpectedLegalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacanciesSummary(Vacancies, 0, 0, 0, 0));
            _commitmentsApi
                .Setup(x => x.GetEmployerAccountSummary(ExpectedAccountId))
                .ReturnsAsync(new List<ApprenticeshipStatusSummary> { new ApprenticeshipStatusSummary {LegalEntityIdentifier = ExpectedLegalEntityCode,ActiveCount = 1} });
            _repository
                .Setup(x => x.GetEmployerAgreementsToRemove(ExpectedAccountId))
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
        public async Task ThenIfTheAgreementIsSignedThenTheRecruitServiceIsCheckedForVacancies()
        {
            //Arrange
            _repository
                .Setup(x => x.GetEmployerAgreementsToRemove(ExpectedAccountId))
                .ReturnsAsync(new List<RemoveEmployerAgreementView>
                {
                    new RemoveEmployerAgreementView {Name = "test company", Status = EmployerAgreementStatus.Pending, Id = ExpectedAgreementId,LegalEntityCode = "Another Code"},
                    new RemoveEmployerAgreementView {Name = "test company", Status = EmployerAgreementStatus.Signed, Id = ExpectedAgreementId,LegalEntityCode = ExpectedLegalEntityCode}
                });
            Vacancies = Enumerable.Repeat(Vacancy, 3);
            _dasRecruitService.Setup(x => x.GetVacanciesByLegalEntity(ExpectedHashedAccountId, ExpectedLegalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacanciesSummary(Vacancies, 0, 0, 0, 0));

            //Act
            var actual = await RequestHandler.Handle(Query);

            Assert.AreEqual(1, actual.Agreements.Count(c => c.HasVacancies));
            _dasRecruitService.Verify(x=>x.GetVacanciesByLegalEntity(ExpectedHashedAccountId,ExpectedLegalId, It.IsAny<CancellationToken>()),Times.AtLeastOnce);
        }

        [Test]
        public async Task ThenIfTheAgreementIsSignedAndNoActiveCommitmentsTheAgreementCanBeRemoved()
        {
            //Arrange
            Vacancies = Enumerable.Empty<Vacancy>();
            _dasRecruitService.Setup(x => x.GetVacanciesByLegalEntity(ExpectedHashedAccountId, ExpectedLegalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacanciesSummary(Vacancies, 0, 0, 0, 0));
            _commitmentsApi
                .Setup(x => x.GetEmployerAccountSummary(ExpectedAccountId))
                .ReturnsAsync(new List<ApprenticeshipStatusSummary>
                {
                    new ApprenticeshipStatusSummary { LegalEntityIdentifier = ExpectedLegalEntityCode, CompletedCount = 1 }
                });
            _repository
                .Setup(x => x.GetEmployerAgreementsToRemove(ExpectedAccountId))
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
        public async Task ThenIfTheAgreementIsSignedAndNoActiveCommitmentsMatchingLegalEntitySourceTheAgreementCanBeRemoved()
        {
            //Arrange
            Vacancies = Enumerable.Empty<Vacancy>();
            _dasRecruitService.Setup(x => x.GetVacanciesByLegalEntity(ExpectedHashedAccountId, ExpectedLegalId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new VacanciesSummary(Vacancies, 0, 0, 0, 0));
            _commitmentsApi
                .Setup(x => x.GetEmployerAccountSummary(ExpectedAccountId))
                .ReturnsAsync(new List<ApprenticeshipStatusSummary>
                {
                    new ApprenticeshipStatusSummary { LegalEntityIdentifier = ExpectedLegalEntityCode, ActiveCount = 1, LegalEntityOrganisationType = Common.Domain.Types.OrganisationType.Charities }
                });
            _repository
                .Setup(x => x.GetEmployerAgreementsToRemove(ExpectedAccountId))
                .ReturnsAsync(new List<RemoveEmployerAgreementView>
                {
                    new RemoveEmployerAgreementView {Name = "test company", Status = EmployerAgreementStatus.Pending, Id = ExpectedAgreementId,LegalEntityCode = "Another Code"},
                    new RemoveEmployerAgreementView {Name = "test company", Status = EmployerAgreementStatus.Signed, Id = ExpectedAgreementId,LegalEntityCode = ExpectedLegalEntityCode, LegalEntitySource = Common.Domain.Types.OrganisationType.Other }
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

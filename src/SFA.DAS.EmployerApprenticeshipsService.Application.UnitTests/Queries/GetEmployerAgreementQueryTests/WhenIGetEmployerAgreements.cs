using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetEmployerAgreement;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain.Data.Repositories;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Domain.Models.AccountTeam;
using SFA.DAS.EAS.Domain.Models.EmployerAgreement;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetEmployerAgreementQueryTests
{
    public class WhenIGetEmployerAgreements : QueryBaseTest<GetEmployerAgreementQueryHandler, GetEmployerAgreementRequest, GetEmployerAgreementResponse>
    {
        private Mock<IEmployerAgreementRepository> _employerAgreementRepository;
        private Mock<IHashingService> _hashingService;
        private Mock<IMembershipRepository> _membershipRepository;
        public override GetEmployerAgreementRequest Query { get; set; }
        public override GetEmployerAgreementQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetEmployerAgreementRequest>> RequestValidator { get; set; }

        private const long ExpectedAgreementId = 123123;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            Query = new GetEmployerAgreementRequest
            {
                ExternalUserId = "ABC123",
                HashedAgreementId = "TYG678",
                HashedAccountId = "DER123"
            };

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(Query.HashedAgreementId)).Returns(ExpectedAgreementId);
            _employerAgreementRepository = new Mock<IEmployerAgreementRepository>();
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedAgreementId)).ReturnsAsync(new EmployerAgreementView {HashedAccountId = Query.HashedAccountId,HashedAgreementId = Query.HashedAgreementId});

            _membershipRepository = new Mock<IMembershipRepository>();
            _membershipRepository.Setup(x => x.GetCaller(Query.HashedAccountId, Query.ExternalUserId)).ReturnsAsync(new MembershipView {FirstName = "Unsigned", LastName = "Tester"});

            RequestHandler = new GetEmployerAgreementQueryHandler( _employerAgreementRepository.Object, _hashingService.Object, RequestValidator.Object, _membershipRepository.Object);
        }

        [Test]
        public void ThenIfTheUserIsNotAuthorizedAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetEmployerAgreementRequest>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async ()=> await RequestHandler.Handle(new GetEmployerAgreementRequest()));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedAgreementId));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsAssignableFrom<GetEmployerAgreementResponse>(actual);
        }

        [Test]
        public async Task ThenIfTheAgreementIsSignedThenTheDetailsArePopulatedInTheResponse()
        {
            //Arrange
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedAgreementId)).ReturnsAsync(new EmployerAgreementView
            {
                HashedAccountId = Query.HashedAccountId,
                HashedAgreementId = Query.HashedAgreementId,
                Status = EmployerAgreementStatus.Signed,
                SignedByName = "Test Tester"
            });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual("Test Tester", actual.EmployerAgreement.SignedByName);
        }

        [Test]
        public async Task ThenIfTheAgreementIsNotSignedThenTheDetailsOfTheUserArePopulatedInTheResponse()
        {
            //Arrange
            _employerAgreementRepository.Setup(x => x.GetEmployerAgreement(ExpectedAgreementId)).ReturnsAsync(new EmployerAgreementView
            {
                HashedAccountId = Query.HashedAccountId,
                HashedAgreementId = Query.HashedAgreementId,
                Status = EmployerAgreementStatus.Pending,
                SignedByName = "Test Tester"
            });

            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.AreEqual("Unsigned Tester",actual.EmployerAgreement.SignedByName);
        }
    }
}

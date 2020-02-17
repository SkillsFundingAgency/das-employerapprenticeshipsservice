using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementRemove;
using SFA.DAS.HashingService;
using SFA.DAS.Testing.EntityFramework;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountEmployerAgreementRemove
{
    public class WhenIGetAccountEmployerAgreementToRemove : QueryBaseTest<GetAccountEmployerAgreementRemoveQueryHandler, GetAccountEmployerAgreementRemoveRequest, GetAccountEmployerAgreementRemoveResponse>
    {
        private Mock<IHashingService> _hashingService;
        private Mock<EmployerAccountsDbContext> _db;
        private EmployerAgreement _employerAgreement;
        private DbSetStub<EmployerAgreement> _emploerAgreementDbSet;
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

            _db = new Mock<EmployerAccountsDbContext>();

            _employerAgreement = new EmployerAgreement { Id = ExpectedAgreementId, AccountLegalEntity = new AccountLegalEntity { Name = ExpectedAgreementName } };
            _emploerAgreementDbSet = new DbSetStub<EmployerAgreement>(_employerAgreement);

            _db.Setup(d => d.Agreements).Returns(_emploerAgreementDbSet);

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedAgreementId)).Returns(ExpectedAgreementId);

            RequestHandler = new GetAccountEmployerAgreementRemoveQueryHandler(RequestValidator.Object, _hashingService.Object, new Lazy<EmployerAccountsDbContext>(() => _db.Object));
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
            _db.Verify(x=>x.Agreements, Times.Once);
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

using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Organisation;
using SFA.DAS.EmployerAccounts.Queries.GetAccountEmployerAgreementRemove;
using SFA.DAS.Hashing;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetOrganisationRemove
{
    public class WhenIGetOrganisationToRemove : QueryBaseTest<GetOrganisationRemoveQueryHandler, GetOrganisationRemoveRequest, GetOrganisationRemoveResponse>
    {
        private Mock<IEmployerAgreementRepository> _repository;
        private Mock<IAccountLegalEntityPublicHashingService> _accountLegalEntityPublicHashingService;

        public override GetOrganisationRemoveRequest Query { get; set; }
        public override GetOrganisationRemoveQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetOrganisationRemoveRequest>> RequestValidator { get; set; }

        private const string ExpectedHashedAccountId = "345ASD";
        private const string ExpectedAccountLegalEntityPublicHashedId = "FGH99";
        private const long ExpectedAccountLegalEntityId = 123456;
        private const string ExpectedUserId = "098GHY";
        private const string ExpectedAgreementName = "Test Company";

        [SetUp]
        public void Arrange()
        {
            SetUp();    

            Query = new GetOrganisationRemoveRequest
            {
                HashedAccountId = ExpectedHashedAccountId,
                AccountLegalEntityPublicHashedId = ExpectedAccountLegalEntityPublicHashedId,
                UserId = ExpectedUserId
            };
            
            _repository = new Mock<IEmployerAgreementRepository>();
            _repository.Setup(x => x.GetAccountLegalEntity(ExpectedAccountLegalEntityId))
                .ReturnsAsync(new AccountLegalEntityModel
                {
                    AccountLegalEntityPublicHashedId = ExpectedAccountLegalEntityPublicHashedId,
                    AccountLegalEntityId = ExpectedAccountLegalEntityId
                });

            _accountLegalEntityPublicHashingService = new Mock<IAccountLegalEntityPublicHashingService>();
            _accountLegalEntityPublicHashingService.Setup(x => x.DecodeValue(ExpectedAccountLegalEntityPublicHashedId)).Returns(ExpectedAccountLegalEntityId);

            RequestHandler = new GetOrganisationRemoveQueryHandler(RequestValidator.Object, _repository.Object, _accountLegalEntityPublicHashingService.Object);
        }

        [Test]
        public void ThenIfTheValidationResultIsUnauthorizedThenAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetOrganisationRemoveRequest>())).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetOrganisationRemoveRequest()));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x=>x.GetAccountLegalEntity(ExpectedAccountLegalEntityId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual.Organisation);
            Assert.AreEqual(ExpectedAccountLegalEntityId, actual.Organisation.AccountLegalEntityId);
            Assert.IsNotNull(ExpectedAgreementName,actual.Organisation.Name);
        }
    }
}

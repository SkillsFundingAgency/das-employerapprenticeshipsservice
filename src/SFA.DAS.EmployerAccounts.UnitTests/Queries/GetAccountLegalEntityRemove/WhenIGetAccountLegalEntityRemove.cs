using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.Organisation;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;
using SFA.DAS.Encoding;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountLegalEntityRemove
{
    public class WhenIGetAccountLegalEntityRemove : QueryBaseTest<GetAccountLegalEntityRemoveQueryHandler, GetAccountLegalEntityRemoveRequest, GetAccountLegalEntityRemoveResponse>
    {
        private Mock<IEncodingService> _encodingService;
        private Mock<IEmployerAgreementRepository> _repository;
        private Mock<ICommitmentsV2ApiClient> _commitmentsV2ApiClient;
      
        public override GetAccountLegalEntityRemoveRequest Query { get; set; }
        public override GetAccountLegalEntityRemoveQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountLegalEntityRemoveRequest>> RequestValidator { get; set; }

        private const string ExpectedHashedAccountId = "345ASD";
        private const string ExpectedHashedAccountLegalEntityId = "PHF78";
        private const string ExpectedHashedAgreementId = "ZH157";
        private const long ExpectedAgreementId = 12345555;
        private const string ExpectedUserId = "098GHY";
        private const long ExpectedAccountId = 98172938;
        private const string ExpectedAccountLegalEntityName = "Test Company";
        private const long ExpectedAccountLegalEntityId = 32453245345;

        [SetUp]
        public void Arrange()
        {
            SetUp();    

            Query = new GetAccountLegalEntityRemoveRequest { HashedAccountId = ExpectedHashedAccountId, HashedAccountLegalEntityId = ExpectedHashedAccountLegalEntityId, UserId = ExpectedUserId};

            _repository = new Mock<IEmployerAgreementRepository>();

            _repository.Setup(r => r.GetAccountLegalEntity(ExpectedAccountLegalEntityId))
                .ReturnsAsync
                (
                    new AccountLegalEntityModel
                    {
                        AccountLegalEntityPublicHashedId = ExpectedHashedAccountLegalEntityId,
                        Name = ExpectedAccountLegalEntityName
                    }
                );

            _repository.Setup(r => r.GetAccountLegalEntityAgreements(ExpectedAccountLegalEntityId))
                .ReturnsAsync
                (
                    new List<EmployerAgreement>()
                );

            _encodingService = new Mock<IEncodingService>();
            _encodingService.Setup(x => x.Decode(ExpectedHashedAccountId, EncodingType.AccountId)).Returns(ExpectedAccountId);
            _encodingService.Setup(x => x.Decode(ExpectedHashedAgreementId, EncodingType.PublicAccountLegalEntityId)).Returns(ExpectedAgreementId);
            _encodingService.Setup(x => x.Decode(ExpectedHashedAccountLegalEntityId, EncodingType.PublicAccountLegalEntityId)).Returns(ExpectedAccountLegalEntityId);

            _commitmentsV2ApiClient = new Mock<ICommitmentsV2ApiClient>();
            _commitmentsV2ApiClient.Setup(x => x.GetEmployerAccountSummary(ExpectedAccountId))
               .ReturnsAsync(new GetApprenticeshipStatusSummaryResponse()
               {
                   ApprenticeshipStatusSummaryResponse = new List<ApprenticeshipStatusSummaryResponse>()  { new ApprenticeshipStatusSummaryResponse() { } }
               });


            RequestHandler = new GetAccountLegalEntityRemoveQueryHandler(
                RequestValidator.Object,
                _repository.Object,
                _encodingService.Object,
                _commitmentsV2ApiClient.Object
            );
        }

        [Test]
        public void ThenIfTheValidationResultIsUnauthorizedThenAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountLegalEntityRemoveRequest>())).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetAccountLegalEntityRemoveRequest(), CancellationToken.None));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            _repository.Verify(x => x.GetAccountLegalEntityAgreements(ExpectedAccountLegalEntityId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query, CancellationToken.None);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(ExpectedAccountLegalEntityName, actual.Name);
        }
    }
}

﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Api.Types.Responses;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Interfaces;
using SFA.DAS.EmployerAccounts.MarkerInterfaces;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Models.Organisation;
using SFA.DAS.EmployerAccounts.Queries.GetAccountLegalEntityRemove;
using SFA.DAS.HashingService;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetAccountLegalEntityRemove
{
    public class WhenIGetAccountLegalEntityRemove : QueryBaseTest<GetAccountLegalEntityRemoveQueryHandler, GetAccountLegalEntityRemoveRequest, GetAccountLegalEntityRemoveResponse>
    {
        private Mock<IHashingService> _hashingService;
        private Mock<IAccountLegalEntityPublicHashingService> _accountLegalEntityPublicHashingService;
        private Mock<IEmployerAgreementRepository> _repository;
        private Mock<ICommitmentsV2ApiClient> _commitmentsV2ApiClient;
      
        public override GetAccountLegalEntityRemoveRequest Query { get; set; }
        public override GetAccountLegalEntityRemoveQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountLegalEntityRemoveRequest>> RequestValidator { get; set; }

        private const string ExpectedHashedAccountId = "345ASD";
        private const string ExpectedHashedAccountLegalEntityId = "PHF78";
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

            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedAccountId)).Returns(ExpectedAccountId);
            _hashingService.Setup(x => x.DecodeValue(ExpectedHashedAccountLegalEntityId)).Returns(ExpectedAgreementId);

            _accountLegalEntityPublicHashingService = new Mock<IAccountLegalEntityPublicHashingService>();
            _accountLegalEntityPublicHashingService.Setup(x => x.DecodeValue(ExpectedHashedAccountLegalEntityId)).Returns(ExpectedAccountLegalEntityId);

            _commitmentsV2ApiClient = new Mock<ICommitmentsV2ApiClient>();
            _commitmentsV2ApiClient.Setup(x => x.GetEmployerAccountSummary(ExpectedAccountId))
               .ReturnsAsync(new GetApprenticeshipStatusSummaryResponse()
               {
                   ApprenticeshipStatusSummaryResponse = new List<ApprenticeshipStatusSummaryResponse>()  { new ApprenticeshipStatusSummaryResponse() { } }
               });


            RequestHandler = new GetAccountLegalEntityRemoveQueryHandler(
                RequestValidator.Object,
                _repository.Object,
                _hashingService.Object,
                _accountLegalEntityPublicHashingService.Object,
                _commitmentsV2ApiClient.Object
            );
        }

        [Test]
        public void ThenIfTheValidationResultIsUnauthorizedThenAnUnauthorizedAccessExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountLegalEntityRemoveRequest>())).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(new GetAccountLegalEntityRemoveRequest()));
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _repository.Verify(x => x.GetAccountLegalEntityAgreements(ExpectedAccountLegalEntityId), Times.Once);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual);
            Assert.AreEqual(ExpectedAccountLegalEntityName, actual.Name);
        }
    }
}

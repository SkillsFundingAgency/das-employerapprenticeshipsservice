using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Application.Queries.GetAccountEmployerAgreements;
using SFA.DAS.EAS.Application.Validation;
using SFA.DAS.EAS.Domain;
using SFA.DAS.EAS.Domain.Data;
using SFA.DAS.EAS.Domain.Interfaces;

namespace SFA.DAS.EAS.Application.UnitTests.Queries.GetAccountEmployerAgreementTests
{
    public class WhenIGetTheAgreements :QueryBaseTest<GetAccountEmployerAgreementsQueryHandler, GetAccountEmployerAgreementsRequest, GetAccountEmployerAgreementsResponse>
    {
        private Mock<IAccountRepository> _accountRepository;
        private Mock<IHashingService> _hashingService;
        public override GetAccountEmployerAgreementsRequest Query { get; set; }
        public override GetAccountEmployerAgreementsQueryHandler RequestHandler { get; set; }
        public override Mock<IValidator<GetAccountEmployerAgreementsRequest>> RequestValidator { get; set; }

        private const string ExpectedExternalUserId = "ABF456";
        private const string ExpectedHashedId = "789RRR";
        private const long ExpectedAccountId = 5456456465;

        [SetUp]
        public void Arrange()
        {
            SetUp();

            _accountRepository = new Mock<IAccountRepository>();
            _accountRepository.Setup(c => c.GetEmployerAgreementsLinkedToAccount(ExpectedAccountId)).ReturnsAsync(new List<EmployerAgreementView>());
            _hashingService = new Mock<IHashingService>();
            _hashingService.Setup(c => c.DecodeValue(ExpectedHashedId)).Returns(ExpectedAccountId);

            Query = new GetAccountEmployerAgreementsRequest { ExternalUserId = ExpectedExternalUserId,HashedId = ExpectedHashedId};

            RequestHandler = new GetAccountEmployerAgreementsQueryHandler(_accountRepository.Object, _hashingService.Object, RequestValidator.Object);
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheRepositoryIsCalled()
        {
            //Act
            await RequestHandler.Handle(Query);

            //Assert
            _accountRepository.Verify(x=>x.GetEmployerAgreementsLinkedToAccount(ExpectedAccountId));
        }

        [Test]
        public void ThenIfTheUserIsNotAuthorizedAnUnauthorizedExceptionIsThrown()
        {
            //Arrange
            RequestValidator.Setup(x => x.ValidateAsync(It.IsAny<GetAccountEmployerAgreementsRequest>())).ReturnsAsync(new ValidationResult {IsUnauthorized = true});

            //Act Assert
            Assert.ThrowsAsync<UnauthorizedAccessException>(async () => await RequestHandler.Handle(Query));
            
        }

        [Test]
        public override async Task ThenIfTheMessageIsValidTheValueIsReturnedInTheResponse()
        {
            //Act
            var actual = await RequestHandler.Handle(Query);

            //Assert
            Assert.IsNotNull(actual.EmployerAgreements);
        }
    }
}

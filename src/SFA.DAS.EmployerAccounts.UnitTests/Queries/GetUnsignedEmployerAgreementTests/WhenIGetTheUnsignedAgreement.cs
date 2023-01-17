using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.HashingService;
using SFA.DAS.Testing.EntityFramework;
using SFA.DAS.Validation;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetUnsignedEmployerAgreementTests
{
    public class WhenIGetTheUnsignedAgreement
    {
        private GetNextUnsignedEmployerAgreementQueryHandler _handler;
        private Mock<IHashingService> _hashingService;
        private Mock<IValidator<GetNextUnsignedEmployerAgreementRequest>> _validator;
        private Mock<EmployerAccountsDbContext> _db;
        private DbSetStub<AccountLegalEntity> _accountLegalEntityDbSet;
        private AccountLegalEntity _accountLegalEntity;

        [SetUp]
        public void Arrange()
        {
            _hashingService = new Mock<IHashingService>();
            _validator = new Mock<IValidator<GetNextUnsignedEmployerAgreementRequest>>();
            _validator.Setup(x => x.ValidateAsync(It.IsAny<GetNextUnsignedEmployerAgreementRequest>())).ReturnsAsync(new ValidationResult());
            _db = new Mock<EmployerAccountsDbContext>();

            _accountLegalEntity = new AccountLegalEntity();
            _accountLegalEntityDbSet = new DbSetStub<AccountLegalEntity>(_accountLegalEntity);

            _db.Setup(d => d.AccountLegalEntities).Returns(_accountLegalEntityDbSet);

            _handler = new GetNextUnsignedEmployerAgreementQueryHandler(new Lazy<EmployerAccountsDbContext>(() => _db.Object), _hashingService.Object, _validator.Object);
        }

        [Test]
        public async Task WhenTheRequestIsInvalidThenAValidationExceptionIsThrown()
        {
            var request = new GetNextUnsignedEmployerAgreementRequest();
            _validator.Setup(x => x.ValidateAsync(request)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "A", "B" }}});

            Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(request, CancellationToken.None));
        }

        [Test]
        public async Task WhenTheRequestIsUnauthorizedThenAnUnauthorizedExceptionIsThrown()
        {
            var request = new GetNextUnsignedEmployerAgreementRequest();
            _validator.Setup(x => x.ValidateAsync(request)).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

            Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));
        }

        [Test]
        public async Task ThenTheHashedAgreementIdIsReturned()
        {
            var accountId = 1234;
            var agreementId = 324345;
            var hashedAgreementId = "ABC345";

            var request = new GetNextUnsignedEmployerAgreementRequest { HashedAccountId = "ABC123" };
            _hashingService.Setup(x => x.DecodeValue(request.HashedAccountId)).Returns(accountId);

            _accountLegalEntity.AccountId = accountId;
            _accountLegalEntity.PendingAgreementId = agreementId;
            _hashingService.Setup(x => x.HashValue(agreementId)).Returns(hashedAgreementId);

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.AreEqual(hashedAgreementId, response.HashedAgreementId);
        }

        [Test]
        public async Task WhenThereIsNoPendingAgreementThenNullIsReturned()
        {
            var accountId = 1234;
            
            var request = new GetNextUnsignedEmployerAgreementRequest { HashedAccountId = "ABC123" };
            _hashingService.Setup(x => x.DecodeValue(request.HashedAccountId)).Returns(accountId);

            _accountLegalEntity.AccountId = accountId;

            var response = await _handler.Handle(request, CancellationToken.None);

            Assert.IsNull(response.HashedAgreementId);
        }
    }
}

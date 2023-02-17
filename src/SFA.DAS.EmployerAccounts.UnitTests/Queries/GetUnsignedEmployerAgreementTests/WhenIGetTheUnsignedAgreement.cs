using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetNextUnsignedEmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries.GetUnsignedEmployerAgreementTests;

public class WhenIGetTheUnsignedAgreement
{
    private GetNextUnsignedEmployerAgreementQueryHandler _handler;
    private Mock<IValidator<GetNextUnsignedEmployerAgreementRequest>> _validator;
    private Mock<EmployerAccountsDbContext> _db;

    private AccountLegalEntity _accountLegalEntity;

    [SetUp]
    public void Arrange()
    {
        _validator = new Mock<IValidator<GetNextUnsignedEmployerAgreementRequest>>();
        _validator.Setup(x => x.ValidateAsync(It.IsAny<GetNextUnsignedEmployerAgreementRequest>())).ReturnsAsync(new ValidationResult());
        _db = new Mock<EmployerAccountsDbContext>();

        _accountLegalEntity = new AccountLegalEntity();

        var accountLegalEntityDbSet = new List<AccountLegalEntity> { _accountLegalEntity }.AsQueryable().BuildMockDbSet();

        _db.Setup(d => d.AccountLegalEntities).Returns(accountLegalEntityDbSet.Object);

        _handler = new GetNextUnsignedEmployerAgreementQueryHandler(new Lazy<EmployerAccountsDbContext>(() => _db.Object), _validator.Object);
    }

    [Test]
    public void WhenTheRequestIsInvalidThenAValidationExceptionIsThrown()
    {
        var request = new GetNextUnsignedEmployerAgreementRequest();
        _validator.Setup(x => x.ValidateAsync(request)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "A", "B" } } });

        Assert.ThrowsAsync<InvalidRequestException>(() => _handler.Handle(request, CancellationToken.None));
    }

    [Test]
    public Task WhenTheRequestIsUnauthorizedThenAnUnauthorizedExceptionIsThrown()
    {
        var request = new GetNextUnsignedEmployerAgreementRequest();
        _validator.Setup(x => x.ValidateAsync(request)).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

        Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(request, CancellationToken.None));

        return Task.CompletedTask;
    }

    [Test]
    public async Task ThenTheAgreementIdIsReturned()
    {
        const int accountId = 1234;
        const int agreementId = 324345;

        var request = new GetNextUnsignedEmployerAgreementRequest { AccountId = accountId };

        _accountLegalEntity.AccountId = accountId;
        _accountLegalEntity.PendingAgreementId = agreementId;

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(agreementId, response.AgreementId);
    }

    [Test]
    public async Task WhenThereIsNoPendingAgreementThenNullIsReturned()
    {
        const int accountId = 1234;

        var request = new GetNextUnsignedEmployerAgreementRequest { AccountId = 1231 };

        _accountLegalEntity.AccountId = accountId;

        var response = await _handler.Handle(request, CancellationToken.None);

        Assert.IsNull(response.AgreementId);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture.NUnit3;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Data.Contracts;
using SFA.DAS.EmployerAccounts.Models.Account;
using SFA.DAS.EmployerAccounts.Queries.GetNextUnsignedEmployerAgreement;
using SFA.DAS.EmployerAccounts.Queries.GetUnsignedEmployerAgreement;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;
using SFA.DAS.Testing.AutoFixture;

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

        //_handler = new GetNextUnsignedEmployerAgreementQueryHandler(new Lazy<EmployerAccountsDbContext>(() => _db.Object), _validator.Object);
    }

    [Test, MoqAutoData]
    public void WhenTheRequestIsInvalidThenAValidationExceptionIsThrown(
        [Frozen]Mock<IValidator<GetNextUnsignedEmployerAgreementRequest>> validatorMock,
        GetNextUnsignedEmployerAgreementRequest request,
        GetNextUnsignedEmployerAgreementQueryHandler handler)
    {
        validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(new ValidationResult { ValidationDictionary = new Dictionary<string, string> { { "A", "B" } } });

        Assert.ThrowsAsync<InvalidRequestException>(() => handler.Handle(request, CancellationToken.None));
    }

    [Test, MoqAutoData]
    public Task WhenTheRequestIsUnauthorizedThenAnUnauthorizedExceptionIsThrown(
        [Frozen] Mock<IValidator<GetNextUnsignedEmployerAgreementRequest>> validatorMock,
        GetNextUnsignedEmployerAgreementRequest request,
        GetNextUnsignedEmployerAgreementQueryHandler handler)
    {
        validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(new ValidationResult { IsUnauthorized = true });

        Assert.ThrowsAsync<UnauthorizedAccessException>(() => handler.Handle(request, CancellationToken.None));

        return Task.CompletedTask;
    }

    [Test, MoqAutoData]
    public async Task ThenTheAgreementIdIsReturned(
        [Frozen] Mock<IEmployerAccountsDbContext> employersDbContext,
        [Frozen] Mock<IValidator<GetNextUnsignedEmployerAgreementRequest>> validatorMock,
        GetNextUnsignedEmployerAgreementRequest request,
        GetNextUnsignedEmployerAgreementQueryHandler handler)
    {
        var accountLegalEntity = new AccountLegalEntity();
        accountLegalEntity.AccountId = request.AccountId;
        accountLegalEntity.PendingAgreementId = 124;
        var accountLegalEntityDbSet = new List<AccountLegalEntity> { accountLegalEntity }.AsQueryable().BuildMockDbSet();
        employersDbContext.Setup(d => d.AccountLegalEntities).Returns(accountLegalEntityDbSet.Object);
        validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(new ValidationResult());

        var response = await handler.Handle(request, CancellationToken.None);

        Assert.AreEqual(accountLegalEntity.PendingAgreementId, response.AgreementId);
    }

    [Test, RecursiveMoqAutoData]
    public async Task WhenThereIsNoPendingAgreementThenNullIsReturned(
        [Frozen] Mock<IEmployerAccountsDbContext> employersDbContext,
        [Frozen] Mock<IValidator<GetNextUnsignedEmployerAgreementRequest>> validatorMock,
        GetNextUnsignedEmployerAgreementRequest request,
        GetNextUnsignedEmployerAgreementQueryHandler handler)
    {
        var accountLegalEntity = new AccountLegalEntity();
        accountLegalEntity.PendingAgreementId = 124;
        var accountLegalEntityDbSet = new List<AccountLegalEntity> { accountLegalEntity }.AsQueryable().BuildMockDbSet();
        employersDbContext.Setup(d => d.AccountLegalEntities).Returns(accountLegalEntityDbSet.Object);
        validatorMock.Setup(x => x.ValidateAsync(request)).ReturnsAsync(new ValidationResult());

        var response = await handler.Handle(request, CancellationToken.None);

        Assert.IsNull(response.AgreementId);
    }
}
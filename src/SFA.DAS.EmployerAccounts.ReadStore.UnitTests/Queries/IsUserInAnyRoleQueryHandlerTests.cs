using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.CosmosDb.Testing;
using SFA.DAS.EmployerAccounts.ReadStore.Application.Queries;
using SFA.DAS.EmployerAccounts.ReadStore.Data;
using SFA.DAS.EmployerAccounts.ReadStore.Models;
using SFA.DAS.EmployerAccounts.Types.Models;
using SFA.DAS.Testing;
using SFA.DAS.Testing.Builders;

namespace SFA.DAS.EmployerAccounts.ReadStore.UnitTests.Queries;

[TestFixture]
[Parallelizable]
public class IsUserInAnyRoleQueryHandlerTests : FluentTest<IsUserInAnyRoleQueryHandlerTestsFixture>
{
    [Test]
    public Task Handle_WhenSingleMatchingUserFound_ShouldReturnTrue()
    {
        return TestAsync(f => f.AddSingleMatchingUser(), f => f.Handle(), (f, r) => r.Should().BeTrue());
    }

    [Test]
    public Task Handle_WhenNoMatchingUserFound_ShouldReturnFalse()
    {
        return TestAsync(f => f.Handle(), (f, r) => r.Should().BeFalse());
    }

    [Test]
    public Task Handle_WhenMultipleMatchingUsersFound_ShouldReturnTrue()
    {
        return TestAsync(f => f.AddMultipleMatchingUsers(), f => f.Handle(), (f, r) => r.Should().BeTrue());
    }

    [Test]
    public Task Handle_WhenNonMatchingBecauseOfUserRef_ShouldReturnFalse()
    {
        return TestAsync(f => f.AddNonMatchingOnUserRef(), f => f.Handle(), (f, r) => r.Should().BeFalse());
    }

    [Test]
    public Task Handle_WhenNotMatchingBecauseOfAccountId_ShouldReturnFalse()
    {
        return TestAsync(f => f.AddNonMatchingOnAccountId(), f => f.Handle(), (f, r) => r.Should().BeFalse());
    }
}

public class IsUserInAnyRoleQueryHandlerTestsFixture
{
    internal IsUserInAnyRoleQuery Query { get; set; }
    public CancellationToken CancellationToken { get; set; }
    internal IRequestHandler<IsUserInAnyRoleQuery,bool> Handler { get; set; }
    internal Mock<IAccountUsersRepository> UserRolesRepository { get; set; }
    internal List<AccountUser> AccountUsers { get; set; }


    public IsUserInAnyRoleQueryHandlerTestsFixture()
    {
        Query = new IsUserInAnyRoleQuery(Guid.NewGuid(), 112);
        CancellationToken = CancellationToken.None;

        AccountUsers = new List<AccountUser>();
        UserRolesRepository = new Mock<IAccountUsersRepository>();
        UserRolesRepository.SetupInMemoryCollection(AccountUsers);

        Handler = new IsUserInAnyRoleQueryHandler(UserRolesRepository.Object);
    }

    internal Task<bool> Handle()
    {
        return Handler.Handle(Query, CancellationToken);
    }

    public IsUserInAnyRoleQueryHandlerTestsFixture AddSingleMatchingUser()
    {
        AccountUsers.Add(CreateBasicMatchingAccountUserWithOwnerRole());

        return this;
    }

    public IsUserInAnyRoleQueryHandlerTestsFixture AddMultipleMatchingUsers()
    {
        AccountUsers.Add(CreateBasicMatchingAccountUserWithOwnerRole());
        AccountUsers.Add(CreateBasicMatchingAccountUserWithOwnerRole());

        return this;
    }

    public IsUserInAnyRoleQueryHandlerTestsFixture AddNonMatchingOnUserRef()
    {
        AccountUsers.Add(CreateBasicMatchingAccountUserWithOwnerRole().Set(x => x.UserRef, Guid.NewGuid()));

        return this;
    }

    public IsUserInAnyRoleQueryHandlerTestsFixture AddNonMatchingOnAccountId()
    {
        AccountUsers.Add(CreateBasicMatchingAccountUserWithOwnerRole().Set(x=>x.AccountId, 214));

        return this;
    }

    private AccountUser CreateBasicMatchingAccountUserWithOwnerRole()
    {
        return CreateBasicMatchingAccountUserObject().Set(x => x.Role, UserRole.Owner);
    }

    private AccountUser CreateBasicMatchingAccountUserObject()
    {
        return ObjectActivator.CreateInstance<AccountUser>()
            .Set(x => x.UserRef, Query.UserRef)
            .Set(x => x.AccountId, Query.AccountId);
    }
}
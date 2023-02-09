using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Queries.GetHealthCheck;
using SFA.DAS.EmployerAccounts.TestCommon.DatabaseMock;
using SFA.DAS.EmployerAccounts.UnitTests.Builders;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries;

[TestFixture]
public class GetHealthCheckQueryHandlerTests : Testing.FluentTest<GetHealthCheckQueryHandlerTestsFixture>
{
    [Test]
    public Task Handle_WhenHandlingAGetHealthCheckQuery_ThenShouldReturnAGetHealthCheckQueryResponse()
    {
        return TestAsync(fixture => fixture.Handle(), (fixture, response) =>
        {
            response.Should().NotBeNull();
            response.HealthCheck.Should().NotBeNull().And.Match<HealthCheckDto>(d => d.Id == fixture.HealthChecks[1].Id);
        });
    }
}

public class GetHealthCheckQueryHandlerTestsFixture
{
    public GetHealthCheckQuery GetHealthCheckQuery { get; set; }
    public IRequestHandler<GetHealthCheckQuery, GetHealthCheckQueryResponse> Handler { get; set; }
    public Mock<EmployerAccountsDbContext> Db { get; set; }
    public List<HealthCheck> HealthChecks { get; set; }

    public GetHealthCheckQueryHandlerTestsFixture()
    {
        GetHealthCheckQuery = new GetHealthCheckQuery();
        Db = new Mock<EmployerAccountsDbContext>();

        HealthChecks = new List<HealthCheck>
        {
            new HealthCheckBuilder().WithId(1).Build(),
            new HealthCheckBuilder().WithId(2).Build()
        };
        
        var mockDbSet = HealthChecks.AsQueryable().BuildMockDbSet();
        
        Db.Setup(d => d.HealthChecks).Returns(mockDbSet.Object);

        Handler = new GetHealthCheckQueryHandler(new Lazy<EmployerAccountsDbContext>(() => Db.Object));
    }

    public Task<GetHealthCheckQueryResponse> Handle()
    {
        return Handler.Handle(GetHealthCheckQuery, CancellationToken.None);
    }
}
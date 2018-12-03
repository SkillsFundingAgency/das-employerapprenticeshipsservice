﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using FluentAssertions;
using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Data;
using SFA.DAS.EmployerAccounts.Dtos;
using SFA.DAS.EmployerAccounts.Mappings;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.Queries.GetHealthCheck;
using SFA.DAS.EmployerAccounts.UnitTests.Builders;
using SFA.DAS.Testing;
using SFA.DAS.Testing.EntityFramework;

namespace SFA.DAS.EmployerAccounts.UnitTests.Queries
{

    [TestFixture]
    public class GetHealthCheckQueryHandlerTests : FluentTest<GetHealthCheckQueryHandlerTestsFixture>
    {
        [Test]
        public Task Handle_WhenHandlingAGetHealthCheckQuery_ThenShouldReturnAGetHealthCheckQueryResponse()
        {
            return RunAsync(f => f.Handle(), (f, r) =>
            {
                r.Should().NotBeNull();
                r.HealthCheck.Should().NotBeNull().And.Match<HealthCheckDto>(d => d.Id == f.HealthChecks[1].Id);
            });
        }
    }

    public class GetHealthCheckQueryHandlerTestsFixture
    {
        public GetHealthCheckQuery GetHealthCheckQuery { get; set; }
        public IAsyncRequestHandler<GetHealthCheckQuery, GetHealthCheckQueryResponse> Handler { get; set; }
        public Mock<EmployerAccountsDbContext> Db { get; set; }
        public IConfigurationProvider ConfigurationProvider { get; set; }
        public List<HealthCheck> HealthChecks { get; set; }

        public GetHealthCheckQueryHandlerTestsFixture()
        {
            GetHealthCheckQuery = new GetHealthCheckQuery();
            Db = new Mock<EmployerAccountsDbContext>();
            ConfigurationProvider = new MapperConfiguration(c => c.AddProfile<HealthCheckMappings>());

            HealthChecks = new List<HealthCheck>
            {
                new HealthCheckBuilder().WithId(1).Build(),
                new HealthCheckBuilder().WithId(2).Build()
            };

            Db.Setup(d => d.HealthChecks).Returns(new DbSetStub<HealthCheck>(HealthChecks));

            Handler = new GetHealthCheckQueryHandler(new Lazy<EmployerAccountsDbContext>(() => Db.Object), ConfigurationProvider);
        }

        public Task<GetHealthCheckQueryResponse> Handle()
        {
            return Handler.Handle(GetHealthCheckQuery);
        }
    }
}
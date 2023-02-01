using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Messages.Events;
using SFA.DAS.EmployerAccounts.Models;
using SFA.DAS.EmployerAccounts.UnitTests.Builders;
using SFA.DAS.Testing;
using SFA.DAS.UnitOfWork.Context;

namespace SFA.DAS.EmployerAccounts.UnitTests.Models;

[TestFixture]
public class HealthCheckTests : FluentTest<HealthCheckTestsFixture>
{
    [Test]
    public void New_WhenCreatingAHealthCheck_ThenShouldCreateAHealthCheck()
    {
        Test(f => f.New(), (f, r) => r.Should().NotBeNull().And.Match<HealthCheck>(h => h.UserRef == f.UserRef));
    }

    [Test]
    public Task Run_WhenRunningAHealthCheck_ThenShouldSetSentRequestProperty()
    {
        return TestAsync(f => f.SetHealthCheck(), f => f.Run(), f => f.HealthCheck.Should().Match<HealthCheck>(h => h.SentRequest >= f.PreRun && h.SentRequest <= f.PostRun));
    }

    [Test]
    public Task Run_WhenRunningAHealthCheck_ThenShouldSetReceivedResponseProperty()
    {
        return TestAsync(f => f.SetHealthCheck(), f => f.Run(), f => f.HealthCheck.Should().Match<HealthCheck>(h => h.ReceivedResponse >= f.PreRun && h.ReceivedResponse <= f.PostRun));
    }

    [Test]
    public Task Run_WhenRunningAHealthCheckAndAnApiExceptionIsThrown_ThenShouldSetReceivedResponseProperty()
    {
        return TestAsync(f => f.SetHealthCheck().SetApiRequestException(), f => f.Run(), f => f.HealthCheck.Should().Match<HealthCheck>(h => h.ReceivedResponse == null));
    }

    [Test]
    public Task Run_WhenRunningAHealthCheck_ThenShouldSetPublishedEventProperty()
    {
        return TestAsync(f => f.SetHealthCheck(), f => f.Run(), f => f.HealthCheck.Should().Match<HealthCheck>(h => h.PublishedEvent >= f.PreRun && h.PublishedEvent <= f.PostRun));
    }

    [Test]
    public Task Run_WhenRunningAHealthCheck_ThenShouldPublishAHealthCheckEvent()
    {
        return TestAsync(f => f.SetHealthCheck(), f => f.Run(), f =>
        {
            var events = f.UnitOfWorkContext.GetEvents().ToList();

            events.Should().AllBeEquivalentTo(new HealthCheckEvent { Id = f.HealthCheck.Id, Created = f.HealthCheck.PublishedEvent });
        });
    }

    [Test]
    public void ReceiveEvent_WhenReceivingAHealthCheckEvent_ThenShouldSetReceivedEventProperty()
    {
        Test(f => f.SetHealthCheck(), f => f.ReceiveEvent(), f => f.HealthCheck.Should().Match<HealthCheck>(h => h.ReceivedEvent >= f.PreRun && h.ReceivedEvent <= f.PostRun));
    }
}

public class HealthCheckTestsFixture
{
    public Guid UserRef { get; set; }
    public IUnitOfWorkContext UnitOfWorkContext { get; set; }
    public HealthCheck HealthCheck { get; set; }
    public Func<Task> ApiRequest { get; set; }
    public DateTime? PreRun { get; set; }
    public DateTime? PostRun { get; set; }

    public HealthCheckTestsFixture()
    {
        UserRef = Guid.NewGuid();
        UnitOfWorkContext = new UnitOfWorkContext();
        ApiRequest = () => Task.CompletedTask;
    }

    public HealthCheck New()
    {
        return new HealthCheck(UserRef);
    }

    public async Task Run()
    {
        PreRun = DateTime.UtcNow;

        await HealthCheck.Run(ApiRequest);

        PostRun = DateTime.UtcNow;
    }

    public void ReceiveEvent()
    {
        PreRun = DateTime.UtcNow;

        HealthCheck.ReceiveEvent(new HealthCheckEvent());

        PostRun = DateTime.UtcNow;
    }

    public HealthCheckTestsFixture SetHealthCheck()
    {
        HealthCheck = new HealthCheckBuilder().WithId(1).Build();

        return this;
    }

    public HealthCheckTestsFixture SetApiRequestException()
    {
        ApiRequest = () => throw new Exception();

        return this;
    }
}
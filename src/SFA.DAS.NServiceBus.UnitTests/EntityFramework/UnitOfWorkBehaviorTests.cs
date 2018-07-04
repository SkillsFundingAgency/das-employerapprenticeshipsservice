using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.NServiceBus.EntityFramework;

namespace SFA.DAS.NServiceBus.UnitTests.EntityFramework
{
    [TestFixture]
    public class UnitOfWorkBehaviorTests : FluentTest<UnitOfWorkBehaviorTestsFixture>
    {
        [Test]
        public Task Invoke_WhenHandlingALogicalMessage_ThenShouldSetUnitOfWorkContextDbConnectionBeforeNextTask()
        {
            return RunAsync(f => f.Invoke(), f => f.UnitOfWorkContext.Verify(c => c.Set<DbConnection>(null), Times.Once));
        }

        [Test]
        public Task Invoke_WhenHandlingALogicalMessage_ThenShouldSetUnitOfWorkContextDbTransactionBeforeNextTask()
        {
            return RunAsync(f => f.Invoke(), f => f.UnitOfWorkContext.Verify(c => c.Set<DbTransaction>(null), Times.Once));
        }

        [Test]
        public Task Invoke_WhenHandlingALogicalMessage_ThenShouldSaveChangesAfterNextTask()
        {
            return RunAsync(f => f.Invoke(), f => f.Db.Verify(d => d.SaveChangesAsync(), Times.Once));
        }

        [Test]
        public Task Invoke_WhenHandlingALogicalMessage_ThenShouldPublishEventsAfterNextTask()
        {
            return RunAsync(f => f.Invoke(), f => f.Context.PublishedMessages.Select(m => m.Message).ShouldAllBeEquivalentTo(f.Events));
        }
    }

    public class UnitOfWorkBehaviorTestsFixture : FluentTestFixture
    {
        public UnitOfWorkBehavior<DbContextFake> Behavior { get; set; }
        public TestableIncomingLogicalMessageContext Context { get; set; }
        public FakeBuilder Builder { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<DbContextFake> Db { get; set; }
        public List<Event> Events { get; set; }
        public bool NextTaskInvoked { get; set; }

        public UnitOfWorkBehaviorTestsFixture()
        {
            Behavior = new UnitOfWorkBehavior<DbContextFake>();
            Builder = new FakeBuilder();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            Db = new Mock<DbContextFake>();

            Context = new TestableIncomingLogicalMessageContext
            {
                Builder = Builder
            };

            Events = new List<Event>
            {
                new FooEvent(),
                new BarEvent()
            };

            UnitOfWorkContext.Setup(c => c.Set<DbConnection>(null)).Callback<DbConnection>(t =>
            {
                if (NextTaskInvoked)
                    throw new Exception("Set called too late");
            });

            UnitOfWorkContext.Setup(c => c.Set<DbTransaction>(null)).Callback<DbTransaction>(t =>
            {
                if (NextTaskInvoked)
                    throw new Exception("Set called too late");
            });

            UnitOfWorkContext.Setup(c => c.GetEvents()).Returns(() =>
            {
                if (!NextTaskInvoked)
                    throw new Exception("GetEvents called too early");

                return Events;
            });

            Db.Setup(d => d.SaveChangesAsync()).ReturnsAsync(1).Callback(() =>
            {
                if (!NextTaskInvoked)
                    throw new Exception("SaveChanges called too early");
            });

            Builder.Register(UnitOfWorkContext.Object);
            Builder.Register(Db.Object);
        }

        public Task Invoke()
        {
            return Behavior.Invoke(Context, () =>
            {
                NextTaskInvoked = true;

                return Task.CompletedTask;
            });
        }
    }
}
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Linq;
using FluentAssertions;
using Moq;
using Moq.Protected;
using NServiceBus;
using NServiceBus.Testing;
using NUnit.Framework;
using SFA.DAS.NServiceBus.EntityFramework;

namespace SFA.DAS.NServiceBus.UnitTests.EntityFramework
{
    [TestFixture]
    public class UnitOfWorkManagerTests : FluentTest<UnitOfWorkManagerTestsFixture>
    {
        [Test]
        public void Begin_WhenBeginningAUnitOfWork_ThenShouldOpenConnection()
        {
            Run(f => f.Begin(), f => f.Connection.Verify(c => c.Open(), Times.Once()));
        }

        [Test]
        public void Begin_WhenBeginningAUnitOfWork_ThenShouldBeginTransaction()
        {
            Run(f => f.Begin(), f => f.Connection.Protected().Verify<DbTransaction>("BeginDbTransaction", Times.Once(), IsolationLevel.Unspecified));
        }

        [Test]
        public void Begin_WhenBeginningAUnitOfWork_ThenShouldSetUnitOfWorkContextDbConnection()
        {
            Run(f => f.Begin(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.Connection.Object), Times.Once()));
        }

        [Test]
        public void Begin_WhenBeginningAUnitOfWork_ThenShouldSetUnitOfWorkContextDbTransaction()
        {
            Run(f => f.Begin(), f => f.UnitOfWorkContext.Verify(c => c.Set(f.Transaction.Object), Times.Once()));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldSaveChangesBeforeAddingOutboxMessage()
        {
            Run(f => f.SetupSaveChangesBeforeAddingOutboxMessage(), f => f.BeginThenEnd(), f => f.Db.Verify(d => d.SaveChangesAsync()));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldNotSaveChanges()
        {
            Run(f => f.BeginThenEndAfterException(), f => f.Db.Verify(d => d.SaveChangesAsync(), Times.Never));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldAddOutboxMessage()
        {
            Run(f => f.SetEvents(), f => f.BeginThenEnd(), f => f.Db.Verify(d => d.OutboxMessages.Add(It.Is<OutboxMessage>(m => !string.IsNullOrWhiteSpace(m.Data))), Times.Once));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterNoChanges_ThenShouldNotAddOutboxMessage()
        {
            Run(f => f.BeginThenEnd(), f => f.Db.Verify(d => d.OutboxMessages.Add(It.IsAny<OutboxMessage>()), Times.Never));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldSaveChangesAfterAddingOutboxMessage()
        {
            Run(f => f.SetEvents().SetupSaveChangesAfterAddingOutboxMessage(), f => f.BeginThenEnd(), f => f.Db.Verify(d => d.SaveChangesAsync()));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldCommitTransaction()
        {
            Run(f => f.BeginThenEnd(), f => f.Transaction.Verify(t => t.Commit()));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldCommitTransaction()
        {
            Run(f => f.BeginThenEndAfterException(), f => f.Transaction.Verify(t => t.Commit(), Times.Never));
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldSendProcessOutboxMessageCommand()
        {
            Run(f => f.SetEvents(), f => f.BeginThenEnd(), f => f.MessageSession.SentMessages.Should().ContainSingle(m =>
                m.Options.GetMessageId() == f.OutboxMessage.Id.ToString() &&
                m.Message.GetType() == typeof(ProcessOutboxMessageCommand)));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldSendProcessOutboxMessageCommand()
        {
            Run(f => f.SetEvents(), f => f.BeginThenEndAfterException(), f => f.MessageSession.SentMessages.Should().BeEmpty());
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterNoChanges_ThenShouldNotSendProcessOutboxMessageCommand()
        {
            Run(f => f.BeginThenEndAfterException(), f => f.MessageSession.SentMessages.Should().BeEmpty());
        }

        [Test]
        public void End_WhenEndingAUnitOfWork_ThenShouldDisposeTransaction()
        {
            Run(f => f.BeginThenEnd(), f => f.Transaction.Protected().Verify("Dispose", Times.Once(), true));
        }

        [Test]
        public void End_WhenEndingAUnitOfWorkAfterAnException_ThenShouldDisposeTransaction()
        {
            Run(f => f.BeginThenEndAfterException(), f => f.Transaction.Protected().Verify("Dispose", Times.Once(), true));
        }
    }

    public class UnitOfWorkManagerTestsFixture : FluentTestFixture
    {
        public IUnitOfWorkManager UnitOfWorkManager { get; set; }
        public TestableMessageSession MessageSession { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public Mock<DbConnection> Connection { get; set; }
        public Mock<IOutboxDbContext> Db { get; set; }
        public Mock<DbTransaction> Transaction { get; set; }
        public List<Event> Events { get; set; }
        public OutboxMessage OutboxMessage { get; set; }

        public UnitOfWorkManagerTestsFixture()
        {
            MessageSession = new TestableMessageSession();
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            Connection = new Mock<DbConnection>();
            Db = new Mock<IOutboxDbContext>();
            Transaction = new Mock<DbTransaction> { CallBase = true };

            Events = new List<Event>
            {
                new FooEvent(),
                new BarEvent()
            };
            
            Connection.Protected().Setup<DbTransaction>("BeginDbTransaction", IsolationLevel.Unspecified).Returns(Transaction.Object);
            Db.Setup(d => d.OutboxMessages.Add(It.IsAny<OutboxMessage>())).Callback<OutboxMessage>(m => OutboxMessage = m);
            Db.Setup(d => d.OutboxMessages.Local).Returns(() => new ObservableCollection<OutboxMessage> { OutboxMessage });
            Db.Setup(d => d.SaveChangesAsync()).ReturnsAsync(1);

            UnitOfWorkManager = new UnitOfWorkManager(
                MessageSession,
                UnitOfWorkContext.Object,
                new Lazy<DbConnection>(() => Connection.Object),
                new Lazy<IOutboxDbContext>(() => Db.Object));
        }

        public void Begin()
        {
            UnitOfWorkManager.Begin();
        }

        public void BeginThenEnd()
        {
            UnitOfWorkManager.Begin();
            UnitOfWorkManager.End();
        }

        public void BeginThenEndAfterException()
        {
            UnitOfWorkManager.Begin();
            UnitOfWorkManager.End(new Exception());
        }

        public UnitOfWorkManagerTestsFixture SetEvents()
        {
            UnitOfWorkContext.Setup(c => c.GetEvents()).Returns(Events);

            return this;
        }

        public UnitOfWorkManagerTestsFixture SetupSaveChangesBeforeAddingOutboxMessage()
        {
            Db.Setup(d => d.SaveChangesAsync()).ReturnsAsync(1).Callback(() =>
            {
                if (OutboxMessage != null)
                    throw new Exception("SaveChanges called too late");
            });

            return this;
        }

        public UnitOfWorkManagerTestsFixture SetupSaveChangesAfterAddingOutboxMessage()
        {
            var savedChanges = 0;

            Db.Setup(d => d.SaveChangesAsync()).ReturnsAsync(1).Callback(() =>
            {
                savedChanges++;

                if (savedChanges == 2 && OutboxMessage == null)
                    throw new Exception("SaveChanges called too early");
            });

            return this;
        }

        public UnitOfWorkManagerTestsFixture SetupCommitTransactionBeforeSendingProcessOutboxMessageCommand()
        {
            Transaction.Setup(t => t.Commit()).Callback(() =>
            {
                if (MessageSession.SentMessages.Any())
                    throw new Exception("Commit called too early");
            });

            return this;
        }
    }
}
using System;
using System.Collections.Generic;
using FluentAssertions;
using Newtonsoft.Json;
using NUnit.Framework;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class OutboxMessageTests : FluentTest<OutboxMessageTestsFixture>
    {
        [Test]
        public void New_WhenCreatingAnOutboxMessage_ThenShouldCreateOutboxMessage()
        {
            Run(f => f.New(), f => f.OutboxMessage.Should().NotBeNull().And.Match<OutboxMessage>(m =>
                m.Id != Guid.Empty &&
                m.Sent >= f.Now &&
                m.Published == null &&
                m.Data == JsonConvert.SerializeObject(f.Events, new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto })));
        }

        [Test]
        public void Publish_WhenPublishing_ThenShouldPublishOutboxMessage()
        {
            Run(f => f.Publish(), f => f.OutboxMessage.Published.Should().BeAfter(f.Now));
        }

        [Test]
        public void Publish_WhenPublishing_ThenShouldReturnEvents()
        {
            Run(f => f.Publish(), (f, r) => r.ShouldAllBeEquivalentTo(f.Events));
        }

        [Test]
        public void Publish_WhenRepublishing_ThenShouldReturnEvents()
        {
            Run(f => f.Republish(), (f, a) => a.ShouldThrow<Exception>().WithMessage("Requires not already published"));
        }
    }

    public class OutboxMessageTestsFixture : FluentTestFixture
    {
        public DateTime Now { get; set; }
        public List<Event> Events { get; set; }
        public OutboxMessage OutboxMessage { get; set; }

        public OutboxMessageTestsFixture()
        {
            Now = DateTime.UtcNow;

            Events = new List<Event>
            {
                new FooEvent(),
                new BarEvent()
            };
        }

        public void New()
        {
            OutboxMessage = new OutboxMessage(Events);
        }

        public IEnumerable<Event> Publish()
        {
            New();

            return OutboxMessage.Publish();
        }

        public IEnumerable<Event> Republish()
        {
            New();
            OutboxMessage.Publish();

            return OutboxMessage.Publish();
        }
    }
}
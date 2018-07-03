using System;
using System.Threading.Tasks;
using Moq;
using NUnit.Framework;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class EventPublisherTests : FluentTest<EventPublisherTestsFixture>
    {
        [Test]
        public Task Publish_WhenPublishingEvent_ThenShouldAddEventToUnitOfWorkContext()
        {
            return RunAsync(f => f.Publish(), f => f.UnitOfWorkContext.Verify(c => c.AddEvent(f.Event)));
        }
    }

    public class EventPublisherTestsFixture : FluentTestFixture
    {
        public Action<FooEvent> Event { get; set; }
        public Mock<IUnitOfWorkContext> UnitOfWorkContext { get; set; }
        public IEventPublisher EventPublisher { get; set; }

        public EventPublisherTestsFixture()
        {
            Event = e => e.Created = DateTime.Now;
            UnitOfWorkContext = new Mock<IUnitOfWorkContext>();
            EventPublisher = new EventPublisher(UnitOfWorkContext.Object);
        }

        public Task Publish()
        {
            return EventPublisher.Publish(Event);
        }
    }
}
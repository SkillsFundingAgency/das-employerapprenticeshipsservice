using MediatR;
using Moq;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Services;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Services.EventPublisherServiceTests
{
    public abstract class EventPublisherServiceTestsBase
    {
        protected EventPublisher Publisher;
        protected Mock<IMediator> Mediator;

        [SetUp]
        public void Arrange()
        {
            Mediator = new Mock<IMediator>();
            Publisher = new EventPublisher(Mediator.Object);
        }
    }
}

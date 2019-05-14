using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Adapters;
using SFA.DAS.EAS.Portal.Worker.UnitTests.Builders;
using FluentAssertions;
using Moq;
using NServiceBus;
using System;

namespace SFA.DAS.EAS.Portal.UnitTests.Application.Adapters
{
    [TestFixture]
    public class CohortAdapterTests
    {
        private CohortAdapter _sut;
        private Mock<IMessageHandlerContext> _mockMessageHandlerContext;
        private string _messageId;

        public CohortAdapterTests()
        {
            _messageId = Guid.NewGuid().ToString();
            _mockMessageHandlerContext = new Mock<IMessageHandlerContext>();

            _mockMessageHandlerContext
                .Setup(m => m.MessageId)
                .Returns(_messageId);

            _sut = new CohortAdapter();
        }

        [Test]
        public void WhenConvertCalled_ThenTheEventIsMappedToTheCommand()
        {
            // arrange
            CohortApprovalRequestedByProvider @event = new CohortApprovalRequestedByProviderBuilder();

            // act
            var result = _sut.Convert(@event, _mockMessageHandlerContext.Object);

            //assert
            result.AccountId.Should().Be(@event.AccountId);
            result.ProviderId.Should().Be(@event.ProviderId);
            result.CommitmentId.Should().Be(@event.CommitmentId);
        }
    }
}

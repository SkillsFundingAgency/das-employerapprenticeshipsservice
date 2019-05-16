using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Adapters;
using FluentAssertions;
using System;
using SFA.DAS.EAS.Portal.UnitTests.Builders;

namespace SFA.DAS.EAS.Portal.UnitTests.Application.Adapters
{
    [Parallelizable]
    [TestFixture]
    public class CohortAdapterTests
    {
        private CohortAdapter _sut;
        private string _messageId;

        public CohortAdapterTests()
        {
            _messageId = Guid.NewGuid().ToString();
            _sut = new CohortAdapter();
        }

        [Test]
        public void WhenConvertCalled_ThenTheEventIsMappedToTheCommand()
        {
            // arrange
            CohortApprovalRequestedByProvider @event = new CohortApprovalRequestedByProviderBuilder();

            // act
            var result = _sut.Convert(@event);

            //assert
            result.AccountId.Should().Be(@event.AccountId);
            result.ProviderId.Should().Be(@event.ProviderId);
            result.CommitmentId.Should().Be(@event.CommitmentId);
        }
    }
}

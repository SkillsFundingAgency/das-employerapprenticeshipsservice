using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Adapters;
using SFA.DAS.EAS.Portal.UnitTests.Builders;

namespace SFA.DAS.EAS.Portal.UnitTests.Portal.Application.Adapters
{
    [Parallelizable]
    [TestFixture]
    public class CohortAdapterTests
    {
        private readonly CohortAdapter _sut;

        public CohortAdapterTests()
        {
            _sut = new CohortAdapter();
        }

        [Test]
        public void WhenConvertCalled_ThenTheEventIsMappedToTheCommand()
        {
            // arrange
            CohortApprovalRequestedByProvider @event = new CohortApprovalRequestedByProviderBuilder();
            var accountId = @event.AccountId;
            var providerId = @event.ProviderId;
            var commitmentId = @event.CommitmentId;

            // act
            var result = _sut.Convert(@event);

            //assert
            result.AccountId.Should().Be(accountId);
            result.ProviderId.Should().Be(providerId);
            result.CommitmentId.Should().Be(commitmentId);
        }
    }
}

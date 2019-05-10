using NUnit.Framework;
using SFA.DAS.CommitmentsV2.Messages.Events;
using SFA.DAS.EAS.Portal.Application.Adapters;
using SFA.DAS.EAS.Portal.Worker.UnitTests.Builders;
using FluentAssertions;

namespace SFA.DAS.EAS.Portal.UnitTests.Application.Adapters
{
    [TestFixture]
    public class CohortAdapterTests
    {
        private CohortAdapter _sut;

        public CohortAdapterTests()
        {

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

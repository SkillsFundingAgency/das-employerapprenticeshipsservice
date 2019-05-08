using NUnit.Framework;
using SFA.DAS.Commitments.Events;
using SFA.DAS.EmployerAccounts.Adapters;
using SFA.DAS.EmployerAccounts.UnitTests.Builders;

namespace SFA.DAS.EmployerAccounts.UnitTests.Adapters
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
            CohortApprovalRequestedByProvider @event = new ApprovalRequestedBuilder();

            // act
            var result = _sut.Convert(@event);

            //assert
            // TODO :
        }
    }
}

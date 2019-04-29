using NUnit.Framework;
using SFA.DAS.EmployerAccounts.Adapters;
using SFA.DAS.EmployerAccounts.Events.Cohort;
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
            CohortCreated @event = new CohortCreatedBuilder();

            // act
            var result = _sut.Convert(@event);

            //assert
            // TODO :
        }
    }
}

using NUnit.Framework;
using SFA.DAS.LevyAggregationProvider.Worker.Model;
using SFA.DAS.LevyAggregationProvider.Worker.Providers;

namespace SFA.DAS.LevyAggregationProvider.Worker.UnitTests
{
    [TestFixture]
    public class LevyAggregatorTests
    {
        private LevyAggregator _aggregator;

        [SetUp]
        public void Setup()
        {
            _aggregator = new LevyAggregator();
        }

        [Test]
        public void SimpleTest()
        {
            const int accountId = 23;

            var response = _aggregator.BuildAggregate(new SourceData
            {
                AccountId = accountId
            });

            Assert.That(response.AccountId, Is.EqualTo(accountId));
        }
    }
}

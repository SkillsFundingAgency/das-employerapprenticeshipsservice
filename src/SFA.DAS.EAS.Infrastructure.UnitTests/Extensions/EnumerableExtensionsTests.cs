using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Infrastructure.Extensions;
using SFA.DAS.EAS.TestCommon;

namespace SFA.DAS.EAS.Infrastructure.UnitTests.Extensions
{
    [TestFixture]
    public class EnumerableExtensionsTests : FluentTest<EnumerableExtensionsTestsFixure>
    {
        [Test]
        public void Batch_WhenBatching_ThenShouldCreateCorrectNumberOfBatches()
        {
            Run(f => f.CreateItems(1, 100), f => f.Items.Batch(10), (f, b) => b.Select(i => i.Count()).Count().Should().Be(10));
        }

        [Test]
        public void Batch_WhenBatching_ThenShouldCreateCorrectNumberOfItemsPerBatch()
        {
            Run(f => f.CreateItems(1, 100), f => f.Items.Batch(10), (f, b) => b.Select(i => i.Count()).All(i => i == 10).Should().BeTrue());
        }
    }

    public class EnumerableExtensionsTestsFixure : FluentTestFixture
    {
        public List<int> Items { get; set; }

        public void CreateItems(int start, int count)
        {
            Items = Enumerable.Range(start, count).ToList();
        }
    }
}
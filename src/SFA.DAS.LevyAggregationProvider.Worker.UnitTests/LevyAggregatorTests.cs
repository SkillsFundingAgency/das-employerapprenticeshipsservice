using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using SFA.DAS.EmployerApprenticeshipsService.Domain;
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
        public void HandleNotSourceData()
        {
            const int accountId = 23;

            var response = _aggregator.BuildAggregate(new SourceData
            {
                AccountId = accountId,
                Data = new List<SourceDataItem>()
            });

            Assert.That(response.AccountId, Is.EqualTo(accountId));
            Assert.That(response.Data.Count, Is.EqualTo(0));
        }

        [Test]
        public void HandleSingleEmpRefAndSingleSourceItem()
        {
            const int accountId = 23;
            const string empRef = "120/1234567";

            var source = new SourceData
            {
                AccountId = accountId,
                Data = new List<SourceDataItem>
                {
                    new SourceDataItem
                    {
                        Id = 1,
                        EmpRef = empRef,
                        ActivityDate = new DateTime(2016, 7, 14),
                        Amount = 100.0m,
                        EnglishFraction = 0.81m,
                        LevyItemType = LevyItemType.Declaration
                    }
                }
            };

            var response = _aggregator.BuildAggregate(source);

            Assert.That(response.AccountId, Is.EqualTo(accountId));
            Assert.That(response.Data.Count, Is.EqualTo(1));

            var aggregateLine = response.Data[0];
            var sourceItem = source.Data[0];

            Assert.That(response.Data[0].Items.Count, Is.EqualTo(1));

            Assert.That(aggregateLine.Month, Is.EqualTo(sourceItem.ActivityDate.Month));
            Assert.That(aggregateLine.Year, Is.EqualTo(sourceItem.ActivityDate.Year));
            Assert.That(aggregateLine.LevyItemType, Is.EqualTo(sourceItem.LevyItemType));
            Assert.That(aggregateLine.Amount, Is.EqualTo(sourceItem.Amount));
            Assert.That(aggregateLine.Balance, Is.EqualTo(sourceItem.Amount));
        }

        [Test]
        public void HandleSingleEmpRefAndMultipleSourceItemsInSamePeriod()
        {
            const int accountId = 23;
            const string empRef = "120/1234567";

            var source = new SourceData
            {
                AccountId = accountId,
                Data = new List<SourceDataItem>
                {
                    new SourceDataItem
                    {
                        Id = 1,
                        EmpRef = empRef,
                        ActivityDate = new DateTime(2016, 7, 14),
                        Amount = 100.0m,
                        EnglishFraction = 0.81m,
                        LevyItemType = LevyItemType.Declaration
                    },
                    new SourceDataItem
                    {
                        Id = 2,
                        EmpRef = empRef,
                        ActivityDate = new DateTime(2016, 7, 28),
                        Amount = 250.0m,
                        EnglishFraction = 0.85m,
                        LevyItemType = LevyItemType.Declaration
                    }
                }
            };

            var response = _aggregator.BuildAggregate(source);

            Assert.That(response.AccountId, Is.EqualTo(accountId));
            Assert.That(response.Data.Count, Is.EqualTo(1));

            var aggregateLine = response.Data[0];
            var sourceItem = source.Data[0];

            Assert.That(response.Data[0].Items.Count, Is.EqualTo(2));

            Assert.That(aggregateLine.Month, Is.EqualTo(sourceItem.ActivityDate.Month));
            Assert.That(aggregateLine.Year, Is.EqualTo(sourceItem.ActivityDate.Year));
            Assert.That(aggregateLine.LevyItemType, Is.EqualTo(sourceItem.LevyItemType));
            Assert.That(aggregateLine.Amount, Is.EqualTo(source.Data[0].Amount + source.Data[1].Amount));
            Assert.That(aggregateLine.Balance, Is.EqualTo(source.Data[0].Amount + source.Data[1].Amount));
        }

        [Test]
        public void HandleSingleEmpRefAndMultipleSourceItemsInSamePeriodOfDifferentType()
        {
            const int accountId = 23;
            const string empRef = "120/1234567";

            var source = new SourceData
            {
                AccountId = accountId,
                Data = new List<SourceDataItem>
                {
                    new SourceDataItem
                    {
                        Id = 1,
                        EmpRef = empRef,
                        ActivityDate = new DateTime(2016, 7, 14),
                        Amount = 100.0m,
                        EnglishFraction = 0.81m,
                        LevyItemType = LevyItemType.Declaration
                    },
                    new SourceDataItem
                    {
                        Id = 2,
                        EmpRef = empRef,
                        ActivityDate = new DateTime(2016, 7, 28),
                        Amount = 250.0m,
                        EnglishFraction = 0.84m,
                        LevyItemType = LevyItemType.TopUp
                    }
                }
            };

            var response = _aggregator.BuildAggregate(source);

            Assert.That(response.AccountId, Is.EqualTo(accountId));
            Assert.That(response.Data.Count, Is.EqualTo(2));
        }

        [Test]
        public void HandleMultipleEmpRefsAndMultipleSourceItemsInSamePeriod()
        {
            const int accountId = 23;
            const string empRef1 = "120/1234567";
            const string empRef2 = "120/7654321";

            var source = new SourceData
            {
                AccountId = accountId,
                Data = new List<SourceDataItem>
                {
                    new SourceDataItem
                    {
                        Id = 1,
                        EmpRef = empRef1,
                        ActivityDate = new DateTime(2016, 7, 14),
                        Amount = 100.0m,
                        EnglishFraction = 0.81m,
                        LevyItemType = LevyItemType.Declaration
                    },
                    new SourceDataItem
                    {
                        Id = 2,
                        EmpRef = empRef2,
                        ActivityDate = new DateTime(2016, 7, 28),
                        Amount = 250.0m,
                        EnglishFraction = 0.84m,
                        LevyItemType = LevyItemType.Declaration
                    }
                }
            };

            var response = _aggregator.BuildAggregate(source);

            Assert.That(response.AccountId, Is.EqualTo(accountId));
            Assert.That(response.Data.Count, Is.EqualTo(1));

            var aggregateLine = response.Data[0];
            var sourceItem = source.Data[0];

            Assert.That(response.Data[0].Items.Count, Is.EqualTo(2));

            Assert.That(aggregateLine.Month, Is.EqualTo(sourceItem.ActivityDate.Month));
            Assert.That(aggregateLine.Year, Is.EqualTo(sourceItem.ActivityDate.Year));
            Assert.That(aggregateLine.LevyItemType, Is.EqualTo(sourceItem.LevyItemType));
            Assert.That(aggregateLine.Amount, Is.EqualTo(source.Data[0].Amount + source.Data[1].Amount));
            Assert.That(aggregateLine.Balance, Is.EqualTo(source.Data[0].Amount + source.Data[1].Amount));
        }

        [Test]
        public void HandleMixedData()
        {
            const int accountId = 23;
            const string empRef1 = "120/1234567";
            const string empRef2 = "120/7654321";

            var source = new SourceData
            {
                AccountId = accountId,
                Data = new List<SourceDataItem>
                {
                    new SourceDataItem
                    {
                        Id = 1,
                        EmpRef = empRef2,
                        ActivityDate = DateTime.Today.AddMonths(-3),
                        Amount = 250.0m,
                        EnglishFraction = 0.84m,
                        LevyItemType = LevyItemType.Declaration
                    },
                    new SourceDataItem
                    {
                        Id = 2,
                        EmpRef = empRef2,
                        ActivityDate = DateTime.Today.AddMonths(-3),
                        Amount = 25.0m,
                        LevyItemType = LevyItemType.TopUp
                    },
                    new SourceDataItem
                    {
                        Id = 3,
                        EmpRef = empRef1,
                        ActivityDate = DateTime.Today.AddMonths(-2),
                        Amount = 100.0m,
                        EnglishFraction = 0.81m,
                        LevyItemType = LevyItemType.Declaration
                    },
                    new SourceDataItem
                    {
                        Id = 4,
                        EmpRef = empRef1,
                        ActivityDate = DateTime.Today.AddMonths(-2),
                        Amount = 10.0m,
                        LevyItemType = LevyItemType.TopUp
                    },
                    new SourceDataItem
                    {
                        Id = 5,
                        EmpRef = empRef1,
                        ActivityDate = DateTime.Today.AddMonths(-1),
                        Amount = 100.0m,
                        EnglishFraction = 0.81m,
                        LevyItemType = LevyItemType.Declaration
                    },
                    new SourceDataItem
                    {
                        Id = 6,
                        EmpRef = empRef1,
                        ActivityDate = DateTime.Today.AddMonths(-1),
                        Amount = 10.0m,
                        LevyItemType = LevyItemType.TopUp
                    },
                    new SourceDataItem
                    {
                        Id = 7,
                        EmpRef = empRef2,
                        ActivityDate = DateTime.Today.AddMonths(-1),
                        Amount = 250.0m,
                        EnglishFraction = 0.84m,
                        LevyItemType = LevyItemType.Declaration
                    },
                    new SourceDataItem
                    {
                        Id = 8,
                        EmpRef = empRef2,
                        ActivityDate = DateTime.Today.AddMonths(-1),
                        Amount = 25.0m,
                        LevyItemType = LevyItemType.TopUp
                    }
                }
            };

            var response = _aggregator.BuildAggregate(source);

            Assert.That(response.AccountId, Is.EqualTo(accountId));
            Assert.That(response.Data.Count, Is.EqualTo(6));

            foreach (var aggregationItem in response.Data)
            {
                var sourceItems =
                    source.Data.Where(
                        x =>
                            x.ActivityDate.Year == aggregationItem.Year && x.ActivityDate.Month == aggregationItem.Month &&
                            x.LevyItemType == aggregationItem.LevyItemType).ToList();

                Assert.That(aggregationItem.Items.Count, Is.EqualTo(sourceItems.Count));
                Assert.That(aggregationItem.Amount, Is.EqualTo(sourceItems.Sum(x => x.Amount)));
            }

            Assert.That(response.Data.Max(x => x.Balance), Is.EqualTo(source.Data.Sum(x => x.Amount)));
        }
    }
}

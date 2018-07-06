using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using NUnit.Framework;

namespace SFA.DAS.NServiceBus.UnitTests
{
    [TestFixture]
    public class UnitOfWorkContextTests : FluentTest<UnitOfWorkContextTestsFixture>
    {
        [Test]
        public void Get_WhenGettingData_ThenShouldReturnData()
        {
            Run(f => f.SetData(), f => f.GetData(), (f, d) => d.Should().Be(f.Data));
        }

        [Test]
        public void GetEvents_WhenGettingEvents_ThenShouldReturnAllEvents()
        {
            Run(f => f.SetEvents(), f => f.GetEvents(), (f, r) => r.Should().HaveCount(2).And.Match<IEnumerable<Event>>(e =>
                e.ElementAt(0) is FooEvent && e.ElementAt(0).Created == f.Events[0].Created &&
                e.ElementAt(1) is BarEvent && e.ElementAt(1).Created == f.Events[1].Created));
        }

        [Test]
        public void GetEvents_WhenGettingEventsAddedOnSeparateThreadsForSameAsyncFlow_ThenShouldReturnAllEvents()
        {
            Run(f => f.SetEventsOnSeparateThreads(), f => f.GetEvents(), (f, r) => r.Should().HaveCount(2).And
                .Contain(e => e is FooEvent && e.Created == f.Events[0].Created).And
                .Contain(e => e is BarEvent && e.Created == f.Events[1].Created));
        }

        [Test]
        public void GetEvents_WhenGettingEventsAddedOnSeparateThreadsForDifferentAsyncFlows_ThenShouldReturnNoEvents()
        {
            Run(f => f.SetEventsOnSeparateAsyncFlow(), f => f.GetEvents(), (f, r) => r.Should().BeEmpty());
        }
    }

    public class UnitOfWorkContextTestsFixture : FluentTestFixture
    {
        public DateTime Now { get; set; }
        public object Data { get; set; }
        public List<Event> Events { get; set; }
        public IUnitOfWorkContext UnitOfWorkContextInstance { get; set; }

        public UnitOfWorkContextTestsFixture()
        {
            Now = DateTime.UtcNow;
            Data = new object();
            Events = new List<Event>();
            UnitOfWorkContextInstance = new UnitOfWorkContext();
        }

        public object GetData()
        {
            return UnitOfWorkContextInstance.Get<object>();
        }

        public IEnumerable<Event> GetEvents()
        {
            return UnitOfWorkContextInstance.GetEvents();
        }

        public UnitOfWorkContextTestsFixture SetData()
        {
            UnitOfWorkContextInstance.Set(Data);

            return this;
        }

        public UnitOfWorkContextTestsFixture SetEvents()
        {
            var fooEvent = new FooEvent();
            var barEvent = new BarEvent();
            
            Events.Add(fooEvent);
            Events.Add(barEvent);

            UnitOfWorkContextInstance.AddEvent<FooEvent>(e => e.Created = fooEvent.Created);
            UnitOfWorkContext.AddEvent<BarEvent>(e => e.Created = barEvent.Created);

            fooEvent.Created = Now.Date.AddDays(-1);
            barEvent.Created = Now.Date.AddDays(1);

            return this;
        }

        public UnitOfWorkContextTestsFixture SetEventsOnSeparateThreads()
        {
            var fooEvent = new FooEvent();
            var barEvent = new BarEvent();

            Events.Add(fooEvent);
            Events.Add(barEvent);

            var tasks = new List<Task>
            {
                Task.Run(() => UnitOfWorkContextInstance.AddEvent<FooEvent>(e => e.Created = fooEvent.Created)),
                Task.Run(() => UnitOfWorkContext.AddEvent<BarEvent>(e => e.Created = barEvent.Created))
            };

            Task.WhenAll(tasks).GetAwaiter().GetResult();

            fooEvent.Created = Now.Date.AddDays(-1);
            barEvent.Created = Now.Date.AddDays(1);

            return this;
        }

        public UnitOfWorkContextTestsFixture SetEventsOnSeparateAsyncFlow()
        {
            var fooEvent = new FooEvent();
            var barEvent = new BarEvent();

            Events.Add(fooEvent);
            Events.Add(barEvent);

            var tasks = new List<Task>
            {
                Task.Run(() =>
                {
                    IUnitOfWorkContext unitOfWorkContextInstance = new UnitOfWorkContext();

                    unitOfWorkContextInstance.AddEvent<FooEvent>(e => e.Created = fooEvent.Created);
                    UnitOfWorkContext.AddEvent<BarEvent>(e => e.Created = barEvent.Created);
                }),
                Task.Run(() =>
                {
                    IUnitOfWorkContext unitOfWorkContextInstance = new UnitOfWorkContext();

                    unitOfWorkContextInstance.AddEvent<FooEvent>(e => e.Created = fooEvent.Created);
                    UnitOfWorkContext.AddEvent<BarEvent>(e => e.Created = barEvent.Created);
                })
            };

            Task.WhenAll(tasks).GetAwaiter().GetResult();

            fooEvent.Created = Now.Date.AddDays(-1);
            barEvent.Created = Now.Date.AddDays(1);

            return this;
        }
    }
}
using System;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using NUnit.Framework;
using SFA.DAS.EAS.Portal.Client.Types;
using SFA.DAS.EAS.Portal.TypesExtensions;
using SFA.DAS.Testing;

namespace SFA.DAS.EAS.Portal.UnitTests.Worker.TypesExtensions.OrganisationExtensions
{
    [TestFixture, Parallelizable]
    public class GetOrAddCohortTests : FluentTest<GetOrAddCohortTestsFixture>
    {
        [Test]
        public void GetOrAddCohort_WhenCohortNotInOrganisation_ThenShouldCreateAndAddCohortToOrganisation()
        {
            Test(f => f.GetOrAddCohort(), f => f.AssertCohortCreatedAndAddedToOrganisation());
        }

        [Test]
        public void GetOrAddCohort_WhenCohortNotInOrganisation_ThenShouldReturnCreatedCohort()
        {
            Test(f => f.GetOrAddCohort(), (f, r) => f.AssertReturnCreatedCohort(r));
        }

        [Test]
        public void
            GetOrAddCohort_WhenCohortNotInOrganisationAndOnAddActionGiven_ThenShouldCreateAndAddMutatedCohortToOrganisation()
        {
            Test(f => f.ArrangeOnAddActionGiven(), f => f.GetOrAddCohort(),
                f => f.AssertCohortCreatedAndAddedToOrganisation(true));
        }

        [Test]
        public void GetOrAddCohort_WhenCohortNotInOrganisationAndOnAddActionGiven_ThenShouldReturnMutatedCohort()
        {
            Test(f => f.ArrangeOnAddActionGiven(), f => f.GetOrAddCohort(),
                (f, r) => f.AssertReturnCreatedCohort(r, true));
        }

        [Test]
        public void GetOrAddCohort_WhenCohortInOrganisation_ThenShouldNotAddANewCohortToOrganisation()
        {
            Test(f => f.ArrangeCohortInOrganisation(), f => f.GetOrAddCohort(),
                f => f.AssertOriginalCohortsInOrganisation());
        }

        [Test]
        public void GetOrAddCohort_WhenCohortInOrganisation_ThenShouldReturnExistingCohort()
        {
            Test(f => f.ArrangeCohortInOrganisation(), f => f.GetOrAddCohort(),
                (f, r) => f.AssertReturnExistingCohort(r));
        }

        [Test]
        public void
            GetOrAddCohort_WhenCohortInOrganisationAndOnGetActionGiven_ThenShouldMutateExistingCohortInOrganisation()
        {
            Test(f => f.ArrangeCohortInOrganisation().ArrangeOnGetActionGiven(),
                f => f.GetOrAddCohort(),
                (f, r) => f.AssertExistingCohortMutatedInOrganisation());
        }

        [Test]
        public void
            GetOrAddCohort_WhenCohortInOrganisationAndOnGetActionGiven_ThenShouldReturnMutatedExistingCohort()
        {
            Test(f => f.ArrangeCohortInOrganisation().ArrangeOnGetActionGiven(),
                f => f.GetOrAddCohort(),
                (f, r) => f.AssertReturnExistingCohort(r, true));
        }
    }

    public class GetOrAddCohortTestsFixture
    {
        public Organisation Organisation { get; set; }
        public Organisation OriginalOrganisation { get; set; }
        public Fixture Fixture { get; set; }
        public Action<Cohort> OnAdd { get; set; }
        public Action<Cohort> OnGet { get; set; }
        public long CohortId { get; set; }
        public string MutatedCohortReference { get; set; }

        public GetOrAddCohortTestsFixture()
        {
            Fixture = new Fixture();
            CohortId = Fixture.Create<long>();
            Organisation = Fixture.Create<Organisation>();
        }

        public GetOrAddCohortTestsFixture ArrangeCohortInOrganisation()
        {
            Organisation.Cohorts.RandomElement().Id = CohortId.ToString();
            return this;
        }

        public GetOrAddCohortTestsFixture ArrangeOnAddActionGiven()
        {
            OnAdd = Cohort => Cohort.Reference = MutatedCohortReference = Fixture.Create<string>();
            return this;
        }

        public GetOrAddCohortTestsFixture ArrangeOnGetActionGiven()
        {
            OnGet = Cohort => Cohort.Reference = MutatedCohortReference = Fixture.Create<string>();
            return this;
        }

        public Cohort GetOrAddCohort()
        {
            OriginalOrganisation = Organisation.Clone();
            return Organisation.GetOrAddCohort(CohortId, OnAdd, OnGet);
        }

        public void AssertCohortCreatedAndAddedToOrganisation(bool mutated = false)
        {
            Organisation.Cohorts.Should()
                .BeEquivalentTo(OriginalOrganisation.Cohorts.Append(ExpectedCreatedCohort(mutated)));
        }

        public void AssertReturnCreatedCohort(Cohort returnedCohort, bool mutated = false)
        {
            returnedCohort.Should()
                .BeEquivalentTo(ExpectedCreatedCohort(mutated));
        }

        private Cohort ExpectedCreatedCohort(bool mutated)
        {
            var expectedCohort = new Cohort
            {
                Id = CohortId.ToString()
            };
            if (mutated)
                expectedCohort.Reference = MutatedCohortReference;

            return expectedCohort;
        }

        public void AssertOriginalCohortsInOrganisation()
        {
            Organisation.Cohorts.Should().BeEquivalentTo(OriginalOrganisation.Cohorts);
        }

        public void AssertExistingCohortMutatedInOrganisation()
        {
            ExpectedExistingCohort(true);
            Organisation.Cohorts.Should()
                .BeEquivalentTo(OriginalOrganisation.Cohorts);
        }

        public void AssertReturnExistingCohort(Cohort returnedCohort, bool mutated = false)
        {
            returnedCohort.Should()
                .BeEquivalentTo(ExpectedExistingCohort(mutated));
        }

        private Cohort ExpectedExistingCohort(bool mutated = false)
        {
            var expectedExistingCohort = OriginalOrganisation.Cohorts.Single(o => o.Id == CohortId.ToString());
            if (mutated)
                expectedExistingCohort.Reference = MutatedCohortReference;

            return expectedExistingCohort;
        }
    }
}
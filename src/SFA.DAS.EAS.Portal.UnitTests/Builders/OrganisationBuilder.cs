using SFA.DAS.EAS.Portal.Client.Types;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.UnitTests.Builders
{
    public class OrganisationBuilder
    {
        private long _id;
        private ICollection<Cohort> _cohorts;

        public OrganisationBuilder()
        {
            var random = new Random();
            _id = random.Next(100, 999);
            _cohorts = new List<Cohort>();
        }

        public Organisation Build()
        {
            return new Organisation
            {                
                Id = _id,                
                Cohorts = _cohorts
            };
        }
      
        public OrganisationBuilder WithId(long id)
        {
            _id = id;
            return this;
        }

        public OrganisationBuilder WithCohort(Cohort cohort)
        {
            _cohorts.Add(cohort);
            return this;
        }

        public static implicit operator Organisation(OrganisationBuilder instance)
        {
            return instance.Build();
        }
    }
}

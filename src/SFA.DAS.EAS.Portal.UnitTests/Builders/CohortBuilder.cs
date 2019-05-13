using SFA.DAS.EAS.Portal.Types;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.UnitTests.Builders
{
    public class CohortBuilder
    {
        private string _id = Guid.NewGuid().ToString();
        private ICollection<Apprenticeship> _apprenticeships;

        public CohortBuilder()
        {
            _apprenticeships = new List<Apprenticeship>();
        }

        public Cohort Build()
        {
            return new Cohort
            {
                Id = _id,
                Apprenticeships = _apprenticeships
            };
        }

        public CohortBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public CohortBuilder WithApprenticeship(Apprenticeship apprenticeship)
        {
            _apprenticeships.Add(apprenticeship);
            return this;
        }

        public static implicit operator Cohort(CohortBuilder instance)
        {
            return instance.Build();
        }
    }
}

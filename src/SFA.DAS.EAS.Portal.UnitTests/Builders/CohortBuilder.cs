using SFA.DAS.EAS.Portal.Client.Types;
using System;
using System.Collections.Generic;

namespace SFA.DAS.EAS.Portal.UnitTests.Builders
{
    public class CohortBuilder
    {
        private string _id = Guid.NewGuid().ToString();
        private string _reference = Guid.NewGuid().ToString();
        private readonly ICollection<Apprenticeship> _apprenticeships;

        public CohortBuilder()
        {
            _apprenticeships = new List<Apprenticeship>();
        }

        public Cohort Build()
        {
            return new Cohort
            {
                Id = _id,
                Reference = _reference,
                Apprenticeships = _apprenticeships
            };
        }

        public CohortBuilder WithId(string id)
        {
            _id = id;
            return this;
        }

        public CohortBuilder WithReference(string reference)
        {
            _reference = reference;
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

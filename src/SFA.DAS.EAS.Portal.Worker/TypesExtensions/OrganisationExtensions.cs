using System;
using System.Linq;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Worker.TypesExtensions
{
    public static class OrganisationExtensions
    {
        public static (Cohort, EntityCreation) GetOrAddCohort(this Organisation organisation, long cohortId)
        {
            //todo: is there a reason for this?
            var cohortIdAsString = cohortId.ToString();
            var cohort = organisation.Cohorts.SingleOrDefault(c => cohortIdAsString.Equals(c.Id, StringComparison.OrdinalIgnoreCase));
            if (cohort == null)
            {
                cohort = new Client.Types.Cohort {Id = cohortIdAsString};
                organisation.Cohorts.Add(cohort);
                return (cohort, EntityCreation.Created);
            }

            return (cohort, EntityCreation.Existed);
        }

    }
}
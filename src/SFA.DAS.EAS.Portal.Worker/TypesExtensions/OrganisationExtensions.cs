using System;
using System.Linq;
using SFA.DAS.EAS.Portal.Client.Types;

namespace SFA.DAS.EAS.Portal.Worker.TypesExtensions
{
    public static class OrganisationExtensions
    {
        public static Cohort GetOrAddCohort(this Organisation organisation, long cohortId, Action<Cohort> onAdd = null, Action<Cohort> onGet = null)
        {
            //todo: is there a reason for this?
            var cohortIdAsString = cohortId.ToString();
            var cohort = organisation.Cohorts.SingleOrDefault(c => cohortIdAsString.Equals(c.Id, StringComparison.OrdinalIgnoreCase));
            if (cohort == null)
            {
                cohort = new Cohort {Id = cohortIdAsString};
                organisation.Cohorts.Add(cohort);
                onAdd?.Invoke(cohort);
            }
            else
            {
                onGet?.Invoke(cohort);
            }
            return cohort;
        }
    }
}
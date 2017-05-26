using System;
using System.Collections.Generic;
using System.Linq;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Web.Extensions;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using ApprenticeshipStatus = SFA.DAS.EAS.Domain.Models.Apprenticeship.ApprenticeshipStatus;
using RecordStatus = SFA.DAS.EAS.Domain.Models.Apprenticeship.RecordStatus;

namespace SFA.DAS.EAS.Web.Orchestrators.Mappers
{
    public class ApprenticeshipFiltersMapper : IApprenticeshipFiltersMapper
    {
        public ApprenticeshipSearchQuery MapToApprenticeshipSearchQuery(ApprenticeshipFiltersViewModel filters)
        {
            var selectedProviders = new List<long>();
            if (filters.Provider != null)
            {
                selectedProviders.AddRange(filters.Provider.Select(long.Parse));
            }

            var selectedStatuses = new List<Commitments.Api.Types.Apprenticeship.Types.ApprenticeshipStatus>();
            if (filters.Status != null)
            {
                selectedStatuses.AddRange(
                    filters.Status.Select(x =>
                        (Commitments.Api.Types.Apprenticeship.Types.ApprenticeshipStatus)
                            Enum.Parse(typeof(Commitments.Api.Types.Apprenticeship.Types.ApprenticeshipStatus), x)));
            }

            var recordStatuses = new List<Commitments.Api.Types.Apprenticeship.Types.RecordStatus>();
            if (filters.RecordStatus != null)
            {
                recordStatuses.AddRange(
                    filters.RecordStatus.Select(
                        x => (Commitments.Api.Types.Apprenticeship.Types.RecordStatus)
                            Enum.Parse(typeof(Commitments.Api.Types.Apprenticeship.Types.RecordStatus), x)));
            }

            var trainingCourses = new List<string>();
            if (filters.Course != null)
            {
                trainingCourses.AddRange(filters.Course);
            }

            var result = new ApprenticeshipSearchQuery
            {
                TrainingProviderIds = selectedProviders,
                ApprenticeshipStatuses = selectedStatuses,
                RecordStatuses = recordStatuses,
                TrainingCourses = trainingCourses
            };

            return result;
        }

        public ApprenticeshipFiltersViewModel Map(Facets facets)
        {
            var result = new ApprenticeshipFiltersViewModel();

            var statuses = new List<KeyValuePair<string, string>>();
            foreach (var status in facets.ApprenticeshipStatuses)
            {
                var key = status.Data.ToString();
                var description = ((ApprenticeshipStatus) status.Data).GetDescription();

                statuses.Add(new KeyValuePair<string, string>(key, description));

                if (status.Selected)
                {
                    result.Status.Add(key);
                }
            }

            var courses = new List<KeyValuePair<string, string>>();
            foreach (var course in facets.TrainingCourses)
            {
                courses.Add(new KeyValuePair<string, string>(course.Data.Id, course.Data.Name));

                if (course.Selected)
                {
                    result.Course.Add(course.Data.Id);
                }
            }

            var providers = new List<KeyValuePair<string, string>>();
            foreach (var provider in facets.TrainingProviders)
            {
                providers.Add(new KeyValuePair<string, string>(provider.Data.Id.ToString(), provider.Data.Name));

                if (provider.Selected)
                {
                    result.Provider.Add(provider.Data.Id.ToString());
                }
            }

            var recordStatuses = new List<KeyValuePair<string, string>>();
            foreach (var recordStatus in facets.RecordStatuses)
            {
                var key = recordStatus.Data.ToString();
                var description = ((RecordStatus) recordStatus.Data).GetDescription();

                recordStatuses.Add(new KeyValuePair<string, string>(key, description));

                if (recordStatus.Selected)
                {
                    result.RecordStatus.Add(recordStatus.Data.ToString());
                }
            }

            result.ApprenticeshipStatusOptions = statuses;
            result.TrainingCourseOptions = courses;
            result.ProviderOrganisationOptions = providers;
            result.RecordStatusOptions = recordStatuses;

            return result;
        }
    }
}
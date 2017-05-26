using System.Collections.Generic;

namespace SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships
{
    public class ApprenticeshipFiltersViewModel
    {
        public ApprenticeshipFiltersViewModel()
        {
            Status = new List<string>();
            RecordStatus = new List<string>();
            Provider = new List<string>();
            Course = new List<string>();

            ApprenticeshipStatusOptions = new List<KeyValuePair<string, string>>();
            TrainingCourseOptions = new List<KeyValuePair<string, string>>();
            RecordStatusOptions = new List<KeyValuePair<string, string>>();
            ProviderOrganisationOptions = new List<KeyValuePair<string, string>>();
        }

        public List<KeyValuePair<string, string>> ApprenticeshipStatusOptions { get; set; }
        public List<KeyValuePair<string, string>> TrainingCourseOptions { get; set; }
        public List<KeyValuePair<string, string>> RecordStatusOptions { get; set; }
        public List<KeyValuePair<string, string>> ProviderOrganisationOptions { get; set; }

        public List<string> Status { get; set; }
        public List<string> RecordStatus { get; set; }
        public List<string> Provider { get; set; }
        public List<string> Course { get; set; }
    }
}
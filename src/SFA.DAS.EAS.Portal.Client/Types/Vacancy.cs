using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Client.Types
{
    class Vacancy
    {
        [JsonProperty("title")]
        public string Title { get; set; }
        [JsonProperty("reference")]
        public long Reference { get; set; }
        [JsonProperty("status")]
        public VacancyStatus Status { get; set; }
        [JsonProperty("closingDate")]
        public DateTime ClosingDate { get; set; }
        [JsonProperty("trainingTitle")]
        public string TrainingTitle { get; set; }
        [JsonProperty("numberOfApplications")]
        public int NumberOfApplications { get; set; }
    }

    internal enum VacancyStatus
    {
        Draft,
        Live,
        Rejected,
        PendingReview,
        Closed

    }
}

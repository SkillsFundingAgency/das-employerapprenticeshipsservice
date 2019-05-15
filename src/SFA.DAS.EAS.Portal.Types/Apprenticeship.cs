using Newtonsoft.Json;
using System;

namespace SFA.DAS.EAS.Portal.Types
{
    public class Apprenticeship
    {
        [JsonProperty("id")]
        public long Id { get; set; }
        [JsonProperty("firstName")]
        public string FirstName { get; set; }
        [JsonProperty("lastName")]
        public string LastName { get; set; }
        [JsonProperty("courseName")]
        public string CourseName { get; set; }
        [JsonProperty("startDate")]
        public DateTime? StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime? EndDate { get; set; }
        [JsonProperty("proposedCost")]
        public decimal? ProposedCost { get; set; }
        [JsonProperty("trainingProvider")]
        public Provider TrainingProvider { get; set; }
    }
}
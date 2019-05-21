using Newtonsoft.Json;
using System;

namespace SFA.DAS.EAS.Portal.Client.Types
{
    public class Reservation
    {
        [JsonProperty("id")]
        public Guid Id { get; set; }
        [JsonProperty("courseName")]
        public string CourseName { get; set; }
        [JsonProperty("courseCode")]
        public string CourseCode { get; set; }
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }        
    }
}
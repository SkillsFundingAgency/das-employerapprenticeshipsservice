using Newtonsoft.Json;
using System;

namespace SFA.DAS.EAS.Portal.Types
{
    public class ReserveFunding
    {
        [JsonProperty("courseName")]
        public string CourseName { get; set; }
        [JsonProperty("courseCode")]
        public string CourseCode { get; set; }
        [JsonProperty("startDate")]
        public DateTime StartDate { get; set; }
        [JsonProperty("endDate")]
        public DateTime EndDate { get; set; }
        [JsonProperty("reservationId")]
        public Guid ReservationId { get; set; }
    }
}
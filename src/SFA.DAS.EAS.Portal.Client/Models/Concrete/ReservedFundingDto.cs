using System;
using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Client.Models.Concrete
{
    public class ReservedFundingDto : IReservedFundingDto
    {
        [JsonProperty("reservationId")]
        public long ReservationId { get; private set; }

        //optional
        [JsonProperty("courseId")]
        public long CourseId { get; private set; }

        //optional
        [JsonProperty("courseName")]
        public string CourseName { get; private set; }

        [JsonProperty("startDate")]
        public DateTime StartDate { get; private set; }

        [JsonProperty("endDate")]
        public DateTime EndDate { get; private set; }
    }
}
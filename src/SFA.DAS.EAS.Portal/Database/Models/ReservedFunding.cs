using Newtonsoft.Json;

namespace SFA.DAS.EAS.Portal.Database.Models
{
    public class ReservedFunding
    {
        [JsonProperty("reservationId")]
        public long ReservationId { get; private set; }

        //optional
        [JsonProperty("courseName")]
        public long CourseName { get; private set; }

        [JsonProperty("startDate")]
        public long StartDate { get; private set; }

        [JsonProperty("endDate")]
        public long EndDate { get; private set; }
    }
}
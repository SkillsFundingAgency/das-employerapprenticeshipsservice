using System;
using Newtonsoft.Json;
using SFA.DAS.EAS.Portal.Client.Models;

namespace SFA.DAS.EAS.Portal.Database.Models
{
    public class ReservedFunding : IReservedFundingDto
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

        public ReservedFunding(long reservationId, long courseId, string courseName, DateTime startDate, DateTime endDate)
        {
            ReservationId = reservationId;
            CourseId = courseId;
            CourseName = courseName;
            StartDate = startDate;
            EndDate = endDate;
        }
        
        [JsonConstructor]
        private ReservedFunding()
        {
        }
    }
}
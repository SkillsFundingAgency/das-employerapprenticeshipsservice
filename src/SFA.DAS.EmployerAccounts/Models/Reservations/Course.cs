using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.Models.Reservations
{
    public class Course
    {
        [JsonProperty("courseId")]
        public string Id { get; set; }
        [JsonProperty("title")]
        public string CourseName { get; set; }
    }
}

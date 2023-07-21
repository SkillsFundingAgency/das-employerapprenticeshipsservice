using Newtonsoft.Json;

namespace SFA.DAS.EmployerAccounts.Models.Reservations;

public class Course
{
    public string CourseId { get; set; }
    public string Title { get; set; }
    public string Level { get; set; }
    [JsonIgnore]
    public string CourseDescription => Title.Equals("UNKNOWN", StringComparison.CurrentCultureIgnoreCase) ? Title : $"{Title} - Level {Level}";
}
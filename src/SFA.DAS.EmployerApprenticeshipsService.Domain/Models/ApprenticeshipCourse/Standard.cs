namespace SFA.DAS.EAS.Domain.Models.ApprenticeshipCourse
{
    public class Standard : ITrainingProgramme
    {
        public string Id { get; set; }
        public long Code { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public Duration Duration { get; set; }
    }
}
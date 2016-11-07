namespace SFA.DAS.EAS.Domain
{
    public class Standard : ITrainingProgramme
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public int Level { get; set; }
        public Duration Duration { get; set; }
    }
}
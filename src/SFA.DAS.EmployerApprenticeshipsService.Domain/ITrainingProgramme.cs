namespace SFA.DAS.EAS.Domain
{
    public interface ITrainingProgramme
    {
        string Id { get; set; }
        string Title { get; set; }

        int Level { get; set; }
    }
}
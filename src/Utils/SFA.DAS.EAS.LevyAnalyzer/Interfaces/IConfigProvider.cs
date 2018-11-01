namespace SFA.DAS.EAS.LevyAnalyzer.Interfaces
{
    public interface IConfigProvider
    {
        TConfigType Get<TConfigType>() where TConfigType : class, new();
    }
}

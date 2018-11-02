namespace SFA.DAS.EAS.LevyAnalyser.Interfaces
{
    /// <summary>
    ///     Returns the specified config type. 
    /// </summary>
    public interface IConfigProvider
    {
        TConfigType Get<TConfigType>() where TConfigType : class, new();
    }
}

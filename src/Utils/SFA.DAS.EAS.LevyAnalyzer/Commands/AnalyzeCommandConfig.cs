namespace SFA.DAS.EAS.LevyAnalyser.Commands
{
    class AnalyzeCommandConfig
    {
        public string AccountIds { get; set; }

        public string SaveQueryResultsFolder { get; set; }

        public bool IgnoreNotFound { get; set; } = true;
    }
}
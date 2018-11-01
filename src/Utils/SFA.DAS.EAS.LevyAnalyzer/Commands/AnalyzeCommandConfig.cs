namespace SFA.DAS.EAS.LevyAnalyzer.Commands
{
    class AnalyzeCommandConfig
    {
        public string AccountIds { get; set; }

        public string SaveQueryResultsFolder { get; set; }

        public bool IgnoreNotFound { get; set; } = true;
    }
}
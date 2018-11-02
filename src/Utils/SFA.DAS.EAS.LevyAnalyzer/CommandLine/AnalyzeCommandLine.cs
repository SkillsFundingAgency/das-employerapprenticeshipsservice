using CommandLine;

namespace SFA.DAS.EAS.LevyAnalyser.CommandLine
{
    [Verb("analyse", HelpText = "Run an aggregate query")]
    public class AnalyseCommandLine : AccountCommandLine
    {
        [Option('r', "rules", HelpText = "List of rules to run", Required = false)]
        public string Rules { get; set; }
    }
}

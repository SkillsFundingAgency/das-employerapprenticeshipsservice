using CommandLine;

namespace SFA.DAS.EAS.LevyAnalyser.CommandLine
{
    public class AccountCommandLine
    {
        [Option('a', "accounts", HelpText = "Account to analyse (in print page format, e.g. 1-5,8,11)")]
        public string AccountIds { get; set; }
    }
}
using CommandLine;

namespace SFA.DAS.EAS.LevyAnalyser.CommandLine
{
    public class ConfigCommandLine
    {
        [Option('c', "config", HelpText = "Specify a config file or folder where the config file can be found.", Required = false)]
        public string ConfigLocation { get; set; }
    }
}
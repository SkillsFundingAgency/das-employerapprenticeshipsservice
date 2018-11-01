using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommandLine;

namespace SFA.DAS.EAS.LevyAnalyzer.CommandLine
{
    [Verb("aggregate", HelpText = "Run an aggregate query")]
    public class AnalyzeCommandLine : AccountCommandLine
    {
        [Option('r', "rules", HelpText = "List of rules to run", Required = false)]
        public string Rules { get; set; }
    }
}

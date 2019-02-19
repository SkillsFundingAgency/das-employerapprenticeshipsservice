using SFA.DAS.EAS.LevyAnalyser.Config;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.ResultSavers;
using StructureMap;

namespace SFA.DAS.EAS.LevyAnalyser.IoC
{
    public class CommonRegistry : Registry
    {
        public CommonRegistry(string configLocation)
        {
            For<IConfigProvider>().Use(new ConfigProvider(configLocation)).Singleton();
            For<IResultSaver>().Use<FileResultSaver>();
            For<ISummarySaver>().Use<FileSummarySaver>();

            Scan(scan =>
            {
                scan.TheCallingAssembly();
                //scan.AssembliesFromApplicationBaseDirectory(assembly => assembly.FullName.Contains("SFA.DAS.EAS.LevyAnalyzer"));
                scan.AddAllTypesOf<ICommand>();

                scan.RegisterConcreteTypesAgainstTheFirstInterface();
            });
        }
    }
}

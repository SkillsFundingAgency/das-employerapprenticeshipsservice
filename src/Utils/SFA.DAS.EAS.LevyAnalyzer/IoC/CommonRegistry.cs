using SFA.DAS.EAS.LevyAnalyzer.Config;
using SFA.DAS.EAS.LevyAnalyzer.Interfaces;
using SFA.DAS.EAS.LevyAnalyzer.ResultSavers;
using StructureMap;

namespace SFA.DAS.EAS.LevyAnalyzer.IoC
{
    public class CommonRegistry : Registry
    {
        public CommonRegistry()
        {
            For<IConfigProvider>().Use<ConfigProvider>().Singleton();
            For<IResultSaver>().Use<FileResultSaver>();

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

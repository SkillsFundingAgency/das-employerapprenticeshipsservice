using SFA.DAS.EAS.LevyAnalyser.Config;
using SFA.DAS.EAS.LevyAnalyser.Interfaces;
using SFA.DAS.EAS.LevyAnalyser.ResultSavers;
using StructureMap;

namespace SFA.DAS.EAS.LevyAnalyser.IoC
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

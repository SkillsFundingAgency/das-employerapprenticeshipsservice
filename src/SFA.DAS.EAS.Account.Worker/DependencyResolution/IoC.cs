using StructureMap;

namespace SFA.DAS.EAS.Account.Worker.DependencyResolution
{
    public static class IoC
    {
        private const string ServiceName = "SFA.DAS.EmployerApprenticeshipsService";

        public static IContainer Initialize()
        {
            return new Container(c =>
            {
                c.AddRegistry<DefaultRegistry>();
            });
        }
    }
}
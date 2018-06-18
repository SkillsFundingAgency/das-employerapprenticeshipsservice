using SFA.DAS.EAS.PaymentUpdater.WebJob.Updater;
using StructureMap;

namespace SFA.DAS.EAS.PaymentUpdater.WebJob.DependencyResolution
{
    public class DefaultRegistry : Registry
    {
        public DefaultRegistry()
        {
            Scan(s =>
            {
                s.AssembliesFromApplicationBaseDirectory(a => a.GetName().Name.StartsWith("SFA.DAS"));
                s.RegisterConcreteTypesAgainstTheFirstInterface();
            });
            
            For<IPaymentProcessor>().Use<PaymentProcessor>();
        }
    }
}
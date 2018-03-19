using StructureMap;

namespace SFA.DAS.EAS.PaymentProvider.Worker.AcceptanceTests
{
    internal class PaymentsProviderWorkerRoleTestWrapper : WorkerRole
    {
        public IContainer TestContainer => Container;
    }
}

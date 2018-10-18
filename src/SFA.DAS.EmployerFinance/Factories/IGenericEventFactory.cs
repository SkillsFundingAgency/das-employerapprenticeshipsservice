using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EmployerFinance.Factories
{
    public interface IGenericEventFactory
    {
        GenericEvent Create<T>(T value);
    }
}

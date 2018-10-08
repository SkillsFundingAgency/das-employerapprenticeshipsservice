using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EmployerAccounts.Events
{
    public interface IGenericEventFactory
    {
        GenericEvent Create<T>(T value);
    }
}

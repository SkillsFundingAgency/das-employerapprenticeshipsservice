using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EmployerAccounts.Factories;

public interface IGenericEventFactory
{
    GenericEvent Create<T>(T value);
}
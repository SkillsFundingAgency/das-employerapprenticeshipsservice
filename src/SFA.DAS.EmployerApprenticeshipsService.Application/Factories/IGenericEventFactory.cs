using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.Factories
{
    public interface IGenericEventFactory
    {
        GenericEvent Create<T>(T value) where T : IEventView;
    }
}

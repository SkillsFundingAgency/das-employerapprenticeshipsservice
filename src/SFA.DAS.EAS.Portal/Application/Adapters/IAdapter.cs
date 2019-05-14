using NServiceBus;

namespace SFA.DAS.EAS.Portal.Application.Adapters
{
    public interface IAdapter<T1, T2>
    {
        T2 Convert(T1 input, IMessageHandlerContext context);
    }
}

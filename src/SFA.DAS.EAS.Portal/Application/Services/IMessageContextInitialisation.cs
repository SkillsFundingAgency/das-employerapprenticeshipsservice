using NServiceBus;

namespace SFA.DAS.EAS.Portal.Application.Services
{
    public interface IMessageContextInitialisation
    {
        void Initialise(IMessageHandlerContext handlerContext);
    }
}
using NServiceBus;

namespace SFA.DAS.EAS.Portal.Application.Services.MessageContext
{
    public interface IMessageContextInitialisation
    {
        void Initialise(IMessageHandlerContext handlerContext);
    }
}
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using SFA.DAS.EAS.Portal.Application.Commands;
using SFA.DAS.EAS.Portal.Events.Reservations;

namespace SFA.DAS.EAS.Portal.Worker.EventHandlers.Reservations
{
    //todo: rename when know real event name
    class ReserveFundingAddedEventHandler : IHandleMessages<ReserveFundingAddedEvent>
    {
        // NServiceBus can't inject an interface message with methods
        private readonly AddReserveFundingCommand _addReserveFundingCommand;

        public ReserveFundingAddedEventHandler(AddReserveFundingCommand addReserveFundingCommand) //, IServiceProvider serviceProvider)
        {
            //todo: fix resolving ILogger event side. it's in the service provider, so unsure why it can't be injected/got
            //var logger = serviceProvider.GetService<ILogger>();
            _addReserveFundingCommand = addReserveFundingCommand;
        }

        public Task Handle(ReserveFundingAddedEvent message, IMessageHandlerContext context)
        {
            return _addReserveFundingCommand.Execute(message, context.MessageId);
        }
    }
}

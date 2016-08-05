using System.Threading.Tasks;
using MediatR;
using NLog;
using SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ProcessNotification;
using SFA.DAS.EmployerApprenticeshipsService.Application.Messages;
using SFA.DAS.EmployerApprenticeshipsService.Domain.Attributes;
using SFA.DAS.Messaging;

namespace SFA.DAS.EAS.Notification.Worker.Providers
{
    public class Notification : INotification
    {
        [QueueName]
        public string send_notification { get; set; }

        private readonly ILogger _logger;
        private readonly IPollingMessageReceiver _pollingMessageReceiver;
        private readonly IMediator _mediator;

        public Notification(ILogger logger, IPollingMessageReceiver pollingMessageReceiver, IMediator mediator)
        {
            _logger = logger;
            _pollingMessageReceiver = pollingMessageReceiver;
            _mediator = mediator;
        }

        public async Task Handle()
        {
            var message = await _pollingMessageReceiver.ReceiveAsAsync<SendNotificationQueueMessage>();

            if (message?.Content != null)
            {
                var messageId = message.Content.Id;

                _logger.Info($"Processing notification id: {messageId}");

                await _mediator.SendAsync(new ProcessNotificationCommand {Id = messageId});

                await message.CompleteAsync();

                _logger.Info($"Completed processing notification id: {messageId}");
            }
        }
    }
}

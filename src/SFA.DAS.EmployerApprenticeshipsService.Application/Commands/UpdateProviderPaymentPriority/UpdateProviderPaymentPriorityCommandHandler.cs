using System;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

using NLog;

using SFA.DAS.EAS.Application.Queries.GetProviderPaymentPriority;

namespace SFA.DAS.EAS.Application.Commands.UpdateProviderPaymentPriority
{
    public class UpdateProviderPaymentPriorityCommandHandler : AsyncRequestHandler<UpdateProviderPaymentPriorityCommand>
    {
        private readonly Validation.IValidator<UpdateProviderPaymentPriorityCommand> _validator;

        private readonly ILogger _logger;

        public UpdateProviderPaymentPriorityCommandHandler(
            Validation.IValidator<UpdateProviderPaymentPriorityCommand> validator,
            ILogger logger)
        {
            _validator = validator;
            _logger = logger;
        }

        protected override Task HandleCore(UpdateProviderPaymentPriorityCommand command)
        {
            var validation = _validator.Validate(command);
            if (!validation.IsValid())
            {
                var traceId = $"UpdatePPP.{DateTime.UtcNow.Ticks}";
                foreach (var entry in validation.ValidationDictionary)
                {
                    _logger.Info($"{entry.Key} -> {entry.Value}, TraceId: {traceId}");
                }
                throw new ValidationException($"Failed validating Provider Payment Priority, TraceId: {traceId}");
            }
            // Log
            // Validate data

            FakePaymentPriorityStore.UpdateData(command.Data.ToList());
            return Task.Run(() => 1);
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using FluentValidation;

using MediatR;

using NLog;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;
using SFA.DAS.Commitments.Api.Types.ProviderPayment;

namespace SFA.DAS.EAS.Application.Commands.UpdateProviderPaymentPriority
{
    public class UpdateProviderPaymentPriorityCommandHandler : AsyncRequestHandler<UpdateProviderPaymentPriorityCommand>
    {
        private readonly Validation.IValidator<UpdateProviderPaymentPriorityCommand> _validator;

        private readonly IEmployerCommitmentApi _commitmentApi;

        private readonly ILogger _logger;

        public UpdateProviderPaymentPriorityCommandHandler(
            Validation.IValidator<UpdateProviderPaymentPriorityCommand> validator,
            IEmployerCommitmentApi commitmentApi,
            ILogger logger)
        {
            _validator = validator;
            _commitmentApi = commitmentApi;
            _logger = logger;
        }

        protected override async Task HandleCore(UpdateProviderPaymentPriorityCommand command)
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

            var submission = CreateProviderPaymentPrioritySubmission(command);

            await _commitmentApi.UpdateCustomProviderPaymentPriority(command.AccountId, submission);
        }

        private static ProviderPaymentPrioritySubmission CreateProviderPaymentPrioritySubmission(UpdateProviderPaymentPriorityCommand command)
        {
            var priorityUpdates =
                command.Data.Select(
                    m => new ProviderPaymentPriorityUpdateItem { ProviderId = m.ProviderId, PriorityOrder = m.PriorityOrder })
                    .ToList();
            var submission = new ProviderPaymentPrioritySubmission
                                 {
                                     Priorities = priorityUpdates,
                                     LastUpdatedByInfo =
                                         new LastUpdateInfo
                                             {
                                                 EmailAddress = command.UserEmailAddress,
                                                 Name = command.UserDisplayName
                                             },
                                     UserId = command.UserId
                                 };
            return submission;
        }
    }
}
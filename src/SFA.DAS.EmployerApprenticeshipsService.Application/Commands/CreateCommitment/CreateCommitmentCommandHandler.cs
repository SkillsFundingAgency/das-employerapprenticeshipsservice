using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json;

using NLog;

using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Application.Commands.SendNotification;
using SFA.DAS.EAS.Domain.Configuration;
using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.Notifications.Api.Types;
using SFA.DAS.Tasks.Api.Client;
using SFA.DAS.Tasks.Api.Types.Templates;

namespace SFA.DAS.EAS.Application.Commands.CreateCommitment
{
    public sealed class CreateCommitmentCommandHandler :
        IAsyncRequestHandler<CreateCommitmentCommand, CreateCommitmentCommandResponse>
    {
        private readonly ICommitmentsApi _commitmentApi;
        private readonly ITasksApi _tasksApi;

        private readonly IMediator _mediator;

        private readonly ILogger _logger;

        private readonly EmployerApprenticeshipsServiceConfiguration _configuration;

        private readonly IHashingService _hashingService;

        private readonly IProviderEmailLookupService _providerEmailLookupService;

        public CreateCommitmentCommandHandler(
            ICommitmentsApi commitmentApi, 
            ITasksApi tasksApi,
            IMediator mediator,
            ILogger logger,
            EmployerApprenticeshipsServiceConfiguration configuration,
            IHashingService hashingService,
            IProviderEmailLookupService providerEmailLookupService)
        {
            if (commitmentApi == null)
                throw new ArgumentNullException(nameof(commitmentApi));
            if (tasksApi == null)
                throw new ArgumentNullException(nameof(tasksApi));
            if (mediator == null)
                throw new ArgumentNullException(nameof(mediator));

            _commitmentApi = commitmentApi;
            _tasksApi = tasksApi;
            _mediator = mediator;
            _logger = logger;
            _configuration = configuration;
            _hashingService = hashingService;
            _providerEmailLookupService = providerEmailLookupService;
        }

        public async Task<CreateCommitmentCommandResponse> Handle(CreateCommitmentCommand request)
        {
            // TODO: This needs to return just the Id
            var commitment = await _commitmentApi.CreateEmployerCommitment(request.Commitment.EmployerAccountId, request.Commitment);

            if (request.Commitment.CommitmentStatus == CommitmentStatus.Active)
            {
                await CreateTask(request, commitment.Id);
            }

            await SendNotification(commitment);

            return new CreateCommitmentCommandResponse { CommitmentId = commitment.Id };
        }

        private async Task SendNotification(Commitment commitment)
        {
            var hashedCommitmentId = _hashingService.HashValue(commitment.Id);
            var emails = await
                _providerEmailLookupService.GetEmailsAsync(
                    commitment.ProviderId.GetValueOrDefault(),
                    commitment.ProviderLastUpdateInfo?.EmailAddress ?? string.Empty);

            _logger.Info($"{emails.Count} provider found email address/es");
            if (!_configuration.CommitmentNotification.SendEmail) return;

            foreach (var email in emails)
            {
                _logger.Info($"Sending email to {email}");
                var notificationCommand = BuildNotificationCommand(
                    email,
                    hashedCommitmentId);
                await _mediator.SendAsync(notificationCommand);
            }
        }

        private SendNotificationCommand BuildNotificationCommand(string email, string hashedCommitmentId)
        {
            return new SendNotificationCommand
            {
                Email = new Email
                {
                    RecipientsAddress = email,
                    TemplateId = _configuration.EmailTemplates.Single(c => c.TemplateType.Equals(EmailTemplateType.CommitmentNotification)).Key,
                    ReplyToAddress = "noreply@sfa.gov.uk",
                    Subject = $"<new Cohort created> {hashedCommitmentId}",
                    SystemId = "x",
                    Tokens = new Dictionary<string, string> {
                        { "type", "review" },
                        { "cohort_reference", hashedCommitmentId }
                    }
                }
            };
        }

        private async Task CreateTask(CreateCommitmentCommand request, long commitmentId)
        {
            var taskTemplate = new CreateCommitmentTemplate
            {
                CommitmentId = commitmentId,
                Message = request.Message,
                Source = $"EMPLOYER-{request.Commitment.EmployerAccountId}"
            };

            var task = new Tasks.Api.Types.Task
            {
                Assignee = $"PROVIDER-{request.Commitment.ProviderId}",
                TaskTemplateId = CreateCommitmentTemplate.TemplateId,
                Name = "Create Commitment",
                Body = JsonConvert.SerializeObject(taskTemplate)
            };

            await this._tasksApi.CreateTask(task.Assignee, task);
        }
    }
}

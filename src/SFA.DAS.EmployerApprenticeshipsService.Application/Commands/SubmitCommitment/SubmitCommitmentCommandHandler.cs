using System.Collections.Generic;
using MediatR;
using Newtonsoft.Json;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;
using SFA.DAS.Tasks.Api.Client;
using SFA.DAS.Tasks.Api.Types.Templates;
using Task = System.Threading.Tasks.Task;

namespace SFA.DAS.EAS.Application.Commands.SubmitCommitment
{
    public sealed class SubmitCommitmentCommandHandler : AsyncRequestHandler<SubmitCommitmentCommand>
    {
        private readonly ICommitmentsApi _commitmentApi;
        private readonly ITasksApi _tasksApi;
        private readonly IEventsApi _eventsApi;
        private readonly SubmitCommitmentCommandValidator _validator;

        public SubmitCommitmentCommandHandler(ICommitmentsApi commitmentApi, ITasksApi tasksApi, IEventsApi eventsApi)
        {
            _commitmentApi = commitmentApi;
            _validator = new SubmitCommitmentCommandValidator();
            _tasksApi = tasksApi;
            _eventsApi = eventsApi;
        }

        protected override async Task HandleCore(SubmitCommitmentCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            var commitment = await _commitmentApi.GetEmployerCommitment(message.EmployerAccountId, message.CommitmentId);

            if (commitment.EmployerAccountId != message.EmployerAccountId)
                throw new InvalidRequestException(new Dictionary<string, string> { { "Commitment", "This commiment does not belong to this Employer Account " } });

            await _commitmentApi.PatchEmployerCommitment(message.EmployerAccountId, message.CommitmentId, message.LastAction);

            await CreateAgreementEvent(message, commitment);

            if (message.CreateTask)
                await CreateTask(message, commitment);
        }

        private async Task CreateTask(SubmitCommitmentCommand message, Commitment commitment)
        {
            var taskTemplate = new CreateCommitmentTemplate
                                   {
                                       CommitmentId = message.CommitmentId,
                                       Message = message.Message,
                                       Source = $"EMPLOYER-{message.EmployerAccountId}"
                                   };

            var task = new Tasks.Api.Types.Task
                           {
                               Assignee = $"PROVIDER-{commitment.ProviderId}",
                               TaskTemplateId = CreateCommitmentTemplate.TemplateId,
                               Name = "Create Commitment",
                               Body = JsonConvert.SerializeObject(taskTemplate)
                           };

            await _tasksApi.CreateTask(task.Assignee, task);
        }

        private async Task CreateAgreementEvent(SubmitCommitmentCommand message, Commitment commitment)
        {
            var agreementEvent = new AgreementEvent
                                     {
                                         EmployerAccountId = message.EmployerAccountId.ToString(),
                                         ProviderId = commitment.ProviderId?.ToString(),
                                         Event = "INITIATED"
                                     };

            await this._eventsApi.CreateAgreementEvent(agreementEvent);
        }
    }
}

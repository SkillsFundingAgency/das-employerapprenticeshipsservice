using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Events.Api.Client;
using SFA.DAS.Events.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.CreateCommitment
{
    public sealed class CreateCommitmentCommandHandler :
        IAsyncRequestHandler<CreateCommitmentCommand, CreateCommitmentCommandResponse>
    {
        private readonly ICommitmentsApi _commitmentApi;
        private readonly IEventsApi _eventsApi;

        public CreateCommitmentCommandHandler(ICommitmentsApi commitmentApi, IEventsApi eventsApi)
        {
            if (commitmentApi == null)
                throw new ArgumentNullException(nameof(commitmentApi));

            _commitmentApi = commitmentApi;
            _eventsApi = eventsApi;
        }

        public async Task<CreateCommitmentCommandResponse> Handle(CreateCommitmentCommand message)
        {
            var agreementEvent = new AgreementEvent
            {
                EmployerAccountId = message.Commitment.EmployerAccountId.ToString(),
                ProviderId = message.Commitment.ProviderId?.ToString(),
                Event = "CREATED"
            };

            await _eventsApi.CreateAgreementEvent(agreementEvent);

            var commitment = await _commitmentApi.CreateEmployerCommitment(message.Commitment.EmployerAccountId, message.Commitment);

            return new CreateCommitmentCommandResponse
            {
                CommitmentId = commitment.Id
            };
        }
    }
}

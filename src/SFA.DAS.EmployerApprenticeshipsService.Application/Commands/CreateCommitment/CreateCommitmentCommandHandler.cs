﻿using System;
using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.CreateCommitment
{
    public sealed class CreateCommitmentCommandHandler : IAsyncRequestHandler<CreateCommitmentCommand, CreateCommitmentCommandResponse>
    {
        private readonly ICommitmentsApi _commitmentApi;

        public CreateCommitmentCommandHandler(ICommitmentsApi commitmentApi)
        {
            if (commitmentApi == null)
                throw new ArgumentNullException(nameof(commitmentApi));

            _commitmentApi = commitmentApi;
        }

        public async Task<CreateCommitmentCommandResponse> Handle(CreateCommitmentCommand message)
        {
            var commitment = await _commitmentApi.CreateEmployerCommitment(message.Commitment.EmployerAccountId, message.Commitment);

            return new CreateCommitmentCommandResponse
            {
                CommitmentId = commitment.Id
            };
        }
    }
}

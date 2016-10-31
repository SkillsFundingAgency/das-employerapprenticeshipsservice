using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;
using SFA.DAS.Commitments.Api.Types;

namespace SFA.DAS.EAS.Application.Commands.ApproveApprenticeship
{
    public sealed class ApproveApprenticeshipCommandHandler : AsyncRequestHandler<ApproveApprenticeshipCommand>
    {
        private ICommitmentsApi _commitmentsApi;
        private readonly ApproveApprenticeshipCommandValidator _validator;

        public ApproveApprenticeshipCommandHandler(ICommitmentsApi commitmentsApi)
        {
            _commitmentsApi = commitmentsApi;
            _validator = new ApproveApprenticeshipCommandValidator();
        }

        protected override async Task HandleCore(ApproveApprenticeshipCommand command)
        {
            var validationResult = _validator.Validate(command);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            // TODO: LWA - Validate Employer is that of the commitment and apprenticeship is in the commitment.
            var commitment = await _commitmentsApi.GetEmployerCommitment(command.EmployerAccountId, command.CommitmentId);

            await _commitmentsApi.PatchEmployerApprenticeship(command.EmployerAccountId, command.CommitmentId, command.ApprenticeshipId, ApprenticeshipStatus.Approved);
        }
    }
}

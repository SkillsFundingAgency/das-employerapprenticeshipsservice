using System.Threading.Tasks;
using MediatR;

using SFA.DAS.Commitments.Api.Client.Interfaces;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.Commitments.Api.Types.Apprenticeship.Types;

namespace SFA.DAS.EAS.Application.Commands.ApproveApprenticeship
{
    public sealed class ApproveApprenticeshipCommandHandler : AsyncRequestHandler<ApproveApprenticeshipCommand>
    {
        private IEmployerCommitmentApi _commitmentsApi;
        private readonly ApproveApprenticeshipCommandValidator _validator;

        public ApproveApprenticeshipCommandHandler(IEmployerCommitmentApi commitmentsApi)
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

            var apprenticeshipSubmission =  new ApprenticeshipSubmission { PaymentStatus = PaymentStatus.Active, UserId = command.UserId };
            await _commitmentsApi.PatchEmployerApprenticeship(command.EmployerAccountId, command.ApprenticeshipId, apprenticeshipSubmission);
        }
    }
}

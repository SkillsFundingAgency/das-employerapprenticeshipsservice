using System.Threading.Tasks;
using MediatR;
using SFA.DAS.Commitments.Api.Client;

namespace SFA.DAS.EmployerApprenticeshipsService.Application.Commands.ApproveApprenticeship
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

        protected override async Task HandleCore(ApproveApprenticeshipCommand message)
        {
            var validationResult = _validator.Validate(message);

            if (!validationResult.IsValid())
                throw new InvalidRequestException(validationResult.ValidationDictionary);

            await _commitmentsApi.PatchApprenticeship(message.EmployerAccountId, message.CommitmentId, message.ApprenticeshipId, Commitments.Api.Types.ApprenticeshipStatus.Approved);
        }
    }
}

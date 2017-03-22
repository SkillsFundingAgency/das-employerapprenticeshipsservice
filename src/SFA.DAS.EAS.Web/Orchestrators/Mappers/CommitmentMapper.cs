using SFA.DAS.EAS.Domain.Interfaces;
using SFA.DAS.EAS.Web.ViewModels;
using System;
using System.Threading.Tasks;

using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.Commitments.Api.Types.Commitment.Types;

namespace SFA.DAS.EAS.Web.Orchestrators.Mappers
{
    public sealed class CommitmentMapper : ICommitmentMapper
    {
        private readonly IHashingService _hashingService;
        private readonly ICommitmentStatusCalculator _statusCalculator;

        public CommitmentMapper(IHashingService hashingService, ICommitmentStatusCalculator statusCalculator)
        {
            if (hashingService == null)
                throw new ArgumentNullException(nameof(hashingService));
            if (statusCalculator == null)
                throw new ArgumentNullException(nameof(statusCalculator));

            _hashingService = hashingService;
            _statusCalculator = statusCalculator;
        }

        public async Task<CommitmentListItemViewModel> MapToCommitmentListItemViewModelAsync(CommitmentListItem commitment, Func<CommitmentListItem, Task<string>> latestMessageFunc)
        {
            var messageTask = latestMessageFunc.Invoke(commitment);

            return new CommitmentListItemViewModel
            {
                HashedCommitmentId = _hashingService.HashValue(commitment.Id),
                Name = commitment.Reference,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName,
                Status = _statusCalculator.GetStatus(commitment.EditStatus, commitment.ApprenticeshipCount, commitment.LastAction, commitment.AgreementStatus),
                ShowViewLink = commitment.EditStatus == EditStatus.EmployerOnly,
                LatestMessage = await messageTask
            };
        }

        public CommitmentViewModel MapToCommitmentViewModel(Commitment commitment)
        {
            return new CommitmentViewModel
            {
                HashedId = _hashingService.HashValue(commitment.Id),
                Name = commitment.Reference,
                LegalEntityName = commitment.LegalEntityName,
                ProviderName = commitment.ProviderName
            };
        }
    }
}
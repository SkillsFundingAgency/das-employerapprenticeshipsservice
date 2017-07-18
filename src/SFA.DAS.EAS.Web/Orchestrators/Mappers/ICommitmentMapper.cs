using System;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types.Commitment;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Orchestrators.Mappers
{
    public interface ICommitmentMapper
    {
        CommitmentViewModel MapToCommitmentViewModel(CommitmentView commitment);
        Task<CommitmentListItemViewModel> MapToCommitmentListItemViewModelAsync(CommitmentListItem commitment, Func<CommitmentListItem, Task<string>> latestMessageFunc);
    }
}
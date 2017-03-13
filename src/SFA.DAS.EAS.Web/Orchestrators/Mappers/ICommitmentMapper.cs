using System;
using System.Threading.Tasks;
using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Web.ViewModels;

namespace SFA.DAS.EAS.Web.Orchestrators.Mappers
{
    public interface ICommitmentMapper
    {
        CommitmentViewModel MapToCommitmentViewModel(Commitment commitment);
        Task<CommitmentListItemViewModel> MapToCommitmentListItemViewModelAsync(CommitmentListItem commitment, Func<CommitmentListItem, Task<string>> latestMessageFunc);
    }
}
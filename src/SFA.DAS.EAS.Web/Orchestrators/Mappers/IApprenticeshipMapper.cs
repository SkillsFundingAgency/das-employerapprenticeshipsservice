using SFA.DAS.Commitments.Api.Types;
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipMapper
    {
        Task<Apprenticeship> MapFromAsync(ApprenticeshipViewModel viewModel);
        ApprenticeshipDetailsViewModel MapToApprenticeshipDetailsViewModel(Apprenticeship apprenticeship);
        ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship);
        ApprenticeshipListItemViewModel MapToApprenticeshipListItem(Apprenticeship apprenticeship);
    }
}
using SFA.DAS.EAS.Web.ViewModels;
using SFA.DAS.Commitments.Api.Types.Apprenticeship;
using SFA.DAS.EAS.Web.ViewModels.ManageApprenticeships;
using System.Threading.Tasks;

namespace SFA.DAS.EAS.Web.Orchestrators.Mappers
{
    public interface IApprenticeshipMapper
    {
        Task<Apprenticeship> MapFromAsync(ApprenticeshipViewModel viewModel);
        ApprenticeshipDetailsViewModel MapToApprenticeshipDetailsViewModel(Apprenticeship apprenticeship);
        ApprenticeshipViewModel MapToApprenticeshipViewModel(Apprenticeship apprenticeship);
        //ApprenticeshipListItemViewModel MapToApprenticeshipListItem(Apprenticeship apprenticeship);
        Task<UpdateApprenticeshipViewModel> CompareAndMapToApprenticeshipViewModel(Apprenticeship original, ApprenticeshipViewModel edited);
    }
}